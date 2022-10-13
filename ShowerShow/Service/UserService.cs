﻿using AutoMapper;
using ShowerShow.DAL;
using ShowerShow.DTO;
using ShowerShow.Models;
using ShowerShow.Repository.Interface;
using ShowerShow.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowerShow.Service
{
    public class UserService: IUserService
    {
        private IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
     
        public async Task CreateUser(CreateUserDTO user)
        {
            await userRepository.CreateUser(user);
        }
        public async Task<GetUserDTO> GetUserById(Guid Id)
        {
            return await userRepository.GetUserById(Id);
        }
        public async Task<bool> CheckIfUserExist(Guid userId)
        {
            return await userRepository.CheckIfUserExist(userId);
        }
        public async Task<List<GetUserDTO>> GetAllFriendsOfUser(Guid userId)
        {
            return await userRepository.GetAllFriendsOfUser(userId);
        }

        public async Task<bool> CheckIfEmailExist(string email)
        {
            return await userRepository.CheckIfEmailExist(email);
        }

        public async Task CreateUserFriend(Guid user1, Guid user2)
        {
             await userRepository.CreateUserFriend(user1, user2);
        }


        public async Task<bool> CheckIfUserIsAlreadyFriend(Guid userId1, Guid userId2)
        {
            return await userRepository.CheckIfUserIsAlreadyFriend(userId1, userId2);
        }
    }
}
