﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RadarrAPI.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20180916230739_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("RadarrAPI.Database.Models.TraktEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<Guid>("State");

                    b.Property<string>("Target")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("State");

                    b.ToTable("Trakt");
                });

            modelBuilder.Entity("RadarrAPI.Database.Models.UpdateEntity", b =>
                {
                    b.Property<int>("UpdateEntityId")
                        .ValueGeneratedOnAdd();

                    b.Property<sbyte>("Branch")
                        .HasColumnType("tinyint");

                    b.Property<string>("FixedStr")
                        .IsRequired()
                        .HasColumnName("Fixed")
                        .HasMaxLength(8192);

                    b.Property<string>("NewStr")
                        .IsRequired()
                        .HasColumnName("New")
                        .HasMaxLength(8192);

                    b.Property<DateTime>("ReleaseDate");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("UpdateEntityId");

                    b.HasIndex("Branch", "Version")
                        .IsUnique();

                    b.ToTable("Updates");
                });

            modelBuilder.Entity("RadarrAPI.Database.Models.UpdateFileEntity", b =>
                {
                    b.Property<int>("UpdateEntityId");

                    b.Property<sbyte>("OperatingSystem")
                        .HasColumnType("tinyint");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("UpdateEntityId", "OperatingSystem");

                    b.ToTable("UpdateFiles");
                });

            modelBuilder.Entity("RadarrAPI.Database.Models.UpdateFileEntity", b =>
                {
                    b.HasOne("RadarrAPI.Database.Models.UpdateEntity", "Update")
                        .WithMany("UpdateFiles")
                        .HasForeignKey("UpdateEntityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
