﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace ShowerShow.Models
{
    public partial class Preferences
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonRequired]
        public Guid UserId { get; set; }
        public AvailableVoices SelectedVoice { get; set; } = AvailableVoices.Default_Male;
        public AvailableLanguages SelectedLanguage { get; set; } = AvailableLanguages.English;


        public AvailableWaterTypes WaterType { get; set; }
        public AvailableCurrencies Currency { get; set; }
        public AvailableThemes Theme { get; set; }
        public AvailableLanguages Language { get; set; }

        public Preferences ReturnDefaultPreference(Guid userId) //queuetrigger will call this method when a new user is created in order to have a default value.
        {
            Preferences preferences = new Preferences()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SelectedLanguage = AvailableLanguages.English,
                SelectedVoice = AvailableVoices.Default_Male,
                WaterType = AvailableWaterTypes.Liters,
                Currency = AvailableCurrencies.EUR,
                Theme = AvailableThemes.Light,

            };
            return preferences;

        }
    }
}
