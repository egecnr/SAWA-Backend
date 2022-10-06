﻿using Newtonsoft.Json;

namespace ShowerShow.Models
{
    internal class ScheduleTag
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public double ActivityDuration { get; set; }

        [JsonRequired]
        public bool IsGasOn { get; set; }

        [JsonRequired]
        public bool IsWaterOn { get; set; }


    }
}
