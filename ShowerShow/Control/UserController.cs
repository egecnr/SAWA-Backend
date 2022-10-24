using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System;
using User = ShowerShow.Models.User;
using CreateUserDTO = ShowerShow.DTO.CreateUserDTO;
using AutoMapper;
using ShowerShow.Repository.Interface;
using ShowerShow.DTO;
using ShowerShow.Models;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShowerShow.Utils;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using ShowerShow.Authorization;
using System.Security.Claims;

namespace ShowerShow.Controllers
{
    public class UserController
    {
        private readonly ILogger<UserController> _logger;
        private IUserService userService;

        public UserController(ILogger<UserController> log,IUserService userService)
        {
            _logger = log;
            this.userService = userService;
        }

        [Function("CreateUser")]
        [OpenApiOperation(operationId: "CreateUser", tags: new[] { "Users" })]
        [OpenApiRequestBody("application/json", typeof(CreateUserDTO), Description = "The user data.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(CreateUserDTO), Description = "The OK response with the new user.")]
        public async Task<HttpResponseData> CreateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/register")] HttpRequestData req)
        {

            _logger.LogInformation("Creating new user.");
            HttpResponseData responseData = req.CreateResponse();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                CreateUserDTO userDTO = JsonConvert.DeserializeObject<CreateUserDTO>(requestBody);
            if(await userService.CheckIfEmailExist(userDTO.Email) || await userService.CheckIfUserNameExist(userDTO.UserName))
            {
                 responseData.StatusCode = HttpStatusCode.BadRequest;
                  return responseData;
            }
            else
            {
                await userService.CreateUser(userDTO);
                responseData.StatusCode = HttpStatusCode.Created;
                
                return responseData;
            }
                
            
        }

        [Function("GetUsersByName")]
        [OpenApiOperation(operationId: "GetUsersByName", tags: new[] { "Users" })]
        [ExampleAuth]
        [OpenApiParameter(name: "userName", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The User ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<GetUserDTO>), Description = "The OK response with the new schedule.")]
        public async Task<HttpResponseData> GetUsersByName([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{userName}")] HttpRequestData req, string userName,FunctionContext functionContext)
        {
            _logger.LogInformation($"Fetching users by name {userName}");
            HttpResponseData responseData = req.CreateResponse();

            //TO DO: make this a better function
            if (AuthCheck.CheckIfUserNotAuthorized(functionContext))
            {
                responseData.StatusCode = HttpStatusCode.Unauthorized;
                return responseData;
            }

            if (!userName.IsNullOrWhiteSpace()) {
                IEnumerable<GetUserDTO> users = await userService.GetUsersByName(userName);
                await responseData.WriteAsJsonAsync(users);
                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;
            }
            else
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }   
        }

        [Function("GetUser")]
        [OpenApiOperation(operationId: "GetUserById", tags: new[] { "Users" })]
        [ExampleAuth]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The user ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetUserDTO), Description = "The OK response with the retrieved user")]
        public async Task<HttpResponseData> GetUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{userId:Guid}")] HttpRequestData req, Guid userId,FunctionContext functionContext)
        {        
                _logger.LogInformation($"Fetching the user by id {userId}");
            HttpResponseData responseData = req.CreateResponse();

            if (AuthCheck.CheckIfUserNotAuthorized(functionContext))
            {
                responseData.StatusCode = HttpStatusCode.Unauthorized;
                return responseData;
            }
            if (await userService.CheckIfUserExistAndActive(userId))
                {
                    GetUserDTO userDTO = await userService.GetUserById(userId);
                    await responseData.WriteAsJsonAsync(userDTO);
                    responseData.StatusCode = HttpStatusCode.OK;
                    return responseData;
                }
                else
                {
                    responseData.StatusCode = HttpStatusCode.NotFound;
                    return responseData;
                }        
        }
        [Function("DeactivateUser")]
        [ExampleAuth]
        [OpenApiOperation(operationId: "DeactivateUser", tags: new[] { "Users" })]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The user ID parameter")]
        [OpenApiParameter(name: "isAccountActive", In = ParameterLocation.Path, Required = true, Type = typeof(bool), Description = "Determines if the account is active or not")]
        public async Task<HttpResponseData> DeactivateUser([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/{userId:Guid}/{isAccountActive:bool}")] HttpRequestData req, Guid userId,bool isAccountActive, FunctionContext functionContext)
        {
            _logger.LogInformation($"Fetching the user by id {userId}");
            HttpResponseData responseData = req.CreateResponse();
            if (AuthCheck.CheckIfUserNotAuthorized(functionContext))
            {
                
                responseData.StatusCode = HttpStatusCode.Unauthorized;
                return responseData;
            }
            if (await userService.CheckIfUserExist(userId))
            {
                
                await userService.DeactivateUserAccount(userId,isAccountActive);
                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;
            }
            else
            {
                responseData.StatusCode = HttpStatusCode.NotFound;
                return responseData;
            }
        }

        [Function("UpdateUser")]
        [OpenApiOperation(operationId: "UpdateUser", tags: new[] { "Users" })]
        [ExampleAuth]
        [OpenApiParameter(name: "userId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The user ID parameter")]
        [OpenApiRequestBody("application/json", typeof(UpdateUserDTO), Description = "The user data to update.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UpdateUserDTO), Description = "The OK response with the updated user")]
        public async Task<HttpResponseData> UpdateUser([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/{userId:Guid}")] HttpRequestData req, Guid userId, FunctionContext functionContext)
        {
            _logger.LogInformation($"Fetching the user by id {userId}");
            HttpResponseData responseData = req.CreateResponse();

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UpdateUserDTO userDTO = JsonConvert.DeserializeObject<UpdateUserDTO>(requestBody);
            if (AuthCheck.CheckIfUserNotAuthorized(functionContext))
            {
                responseData.StatusCode = HttpStatusCode.Unauthorized;
                return responseData;
            }
            if (await userService.CheckIfUserExistAndActive(userId))
            {
                if (!(await userService.CheckIfEmailExist(userId,userDTO.Email)) && !(await userService.CheckIfUserNameExist(userId, userDTO.UserName))) {
                    await userService.UpdateUser(userId, userDTO);
                    responseData.StatusCode = HttpStatusCode.Accepted;
                    return responseData;
                }
                else
                {
                    responseData.StatusCode = HttpStatusCode.BadRequest;
                    return responseData;
                }
               
            }
            else
            {
                responseData.StatusCode = HttpStatusCode.NotFound;
                return responseData;
            }
        }
    }
}

