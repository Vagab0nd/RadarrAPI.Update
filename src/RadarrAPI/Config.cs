﻿using System.Collections.Generic;
using RadarrAPI.Update;

namespace RadarrAPI
{
    public class Config
    {
        public string DataDirectory { get; set; }

        public string Database { get; set; }

        public string AppVeyorApiKey { get; set; }

        public string ApiKey { get; set; }

        public Dictionary<Branch, List<string>> Triggers { get; set; }
    }
}
