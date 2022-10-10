﻿using ShowerShow.Models;
using ShowerShow.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerShow.Service
{
    internal class AchievementService : IAchievementService
    {
        private readonly IAchievementRepository _achievementRepository;

        public AchievementService(IAchievementRepository achievementrepository)
        {
            _achievementRepository = achievementrepository;
        }
        public Task<Achievement> getAchievementById(Guid achievementId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Achievement>> getAchievementsById(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Achievement> updateAchievementById(Guid achievementId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
