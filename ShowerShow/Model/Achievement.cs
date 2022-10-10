﻿using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShowerShow.Models
{
    //This achievement class along with dtos can be changed to fit the right implementation.

    public class Achievement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonRequired]
        public string Title { get; set; }
        [JsonRequired]
        public string Description { get; set; }
        [JsonRequired]
        public int CurrentValue { get; set; } = 0;
        [JsonRequired]
        public int TargetValue { get; set; }
    }
}