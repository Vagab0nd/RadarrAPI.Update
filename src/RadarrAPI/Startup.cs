﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog.Extensions.Logging;
using NLog.Web;
using Octokit;
using RadarrAPI.Database;
using RadarrAPI.Services.BackgroundTasks;
using RadarrAPI.Services.ReleaseCheck;
using RadarrAPI.Services.ReleaseCheck.AppVeyor;
using RadarrAPI.Services.ReleaseCheck.Github;
using StatsdClient;
using TraktApiSharp;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace RadarrAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Config = configuration;
            ConfigRadarr = Config.GetSection("Radarr").Get<Config>();

            env.ConfigureNLog("nlog.config");
            
            SetupDataDirectory();
            SetupDatadog();
        }

        public IConfiguration Config { get; }
        
        public Config ConfigRadarr { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Config>(Config.GetSection("Radarr"));
            services.AddDbContextPool<DatabaseContext>(o => o.UseMySql(ConfigRadarr.Database));

            services.AddSingleton(new GitHubClient(new ProductHeaderValue("RadarrAPI")));
            services.AddHttpClient("AppVeyor")
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    var config = serviceProvider.GetRequiredService<IOptions<Config>>();

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config.Value.AppVeyorApiKey);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    UseCookies = false,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddTransient<ReleaseService>();
            services.AddTransient<GithubReleaseSource>();
            services.AddTransient<AppVeyorReleaseSource>();
            
            services.AddSingleton(new TraktClient(Config.GetSection("Trakt")["ClientId"], Config.GetSection("Trakt")["ClientSecret"]));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            
            applicationLifetime.ApplicationStarted.Register(() => DogStatsd.Event("RadarrAPI", "RadarrAPI just started."));
            applicationLifetime.ApplicationStopped.Register(() => DogStatsd.Event("RadarrAPI", "RadarrAPI just stopped."));
        }

        private void SetupDataDirectory()
        {
            // Check data path
            if (!Path.IsPathRooted(ConfigRadarr.DataDirectory))
            {
                throw new Exception("DataDirectory path must be absolute.");
            }

            // Create
            Directory.CreateDirectory(ConfigRadarr.DataDirectory);
        }

        private void SetupDatadog()
        {
            var server = Config.GetSection("DataDog")["Server"];
            var port = Config.GetSection("DataDog").GetValue<int>("Port");
            var prefix = Config.GetSection("DataDog")["Prefix"];

            if (string.IsNullOrWhiteSpace(server) || port == 0) return;

            DogStatsd.Configure(new StatsdConfig
            {
                StatsdServerName = server,
                StatsdPort = port,
                Prefix = prefix
            });
        }
    }
}