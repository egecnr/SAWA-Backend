﻿using Microsoft.EntityFrameworkCore;
using ShowerShow.DAL;
using ShowerShow.Models;
using ShowerShow.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerShow.Repository
{
    internal class AchievementRepository : IAchievementRepository
    {
        private readonly DatabaseContext _databaseContext;

        public AchievementRepository(DatabaseContext databasecontext)
        {
            _databaseContext = databasecontext;  
        }

        public  Task<Achievement> GetAchievementById(Guid achievementId, Guid userId)
        {
            // this.context.Boards.Where(x => x.UserId == userId).Select(y => y.BoardId);
            var ach = _databaseContext.Users.Where(x => x.Id == userId).Include(y => y.Achievements.Where(z => z.Id == achievementId));
            return (Task<Achievement>)ach;
        }

        public async Task<List<Achievement>> GetAchievementsById(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Achievement> UpdateAchievementById(Guid achievementId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
