﻿using ShowerShow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerShow.Repository.Interfaces
{
    public interface IAchievementRepository
    {
        Task<List<Achievement>> GetAchievementsById(Guid userId); //getting a list of achievement
        Task<Achievement> GetAchievementById(Guid achievementId, Guid userId); //getting a single achievement
        Task<Achievement> UpdateAchievementById(Guid achievementId, Guid userId);
  

    }
}
