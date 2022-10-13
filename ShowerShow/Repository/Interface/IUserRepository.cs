﻿using ShowerShow.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerShow.Repository.Interface
{
    public interface IUserRepository
    {
        public Task<bool> CheckIfUserExist(Guid userId);
        public Task<bool> CheckIfEmailExist(string email);
        public Task CreateUser(CreateUserDTO user);
        public Task CreateUserFriend(Guid user1,Guid user2);
        public Task<GetUserDTO> GetUserById(Guid Id);
        public Task<List<GetUserDTO>> GetAllFriendsOfUser(Guid userId);
        public Task<bool> CheckIfUserIsAlreadyFriend(Guid userId1, Guid userId2);


    }
}
