﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using ShowerShow.Controllers;
using ShowerShow.DTO;
using ShowerShow.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ShowerShow.Service;
using ShowerShow.Repository.Interface;
using ShowerShow.Authorization;

namespace ShowerShow.Control
{
    public class ShowerDataController
    {
        private readonly ILogger<ShowerController> _logger;
        private readonly IShowerDataService showerDataService;
        private readonly IUserService userService;

        public ShowerDataController(ILogger<ShowerController> log, IShowerDataService showerDataService, IUserService userService)
        {
            _logger = log;
            this.showerDataService = showerDataService;
            this.userService = userService;
        }

        [Function(nameof(GetShowerById))]
        [OpenApiOperation(operationId: "getShowerById", tags: new[] { "Shower data" })]
        [ExampleAuth]
        [OpenApiParameter(name: "UserId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The user id parameter")]
        [OpenApiParameter(name: "ShowerId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "id of the requested shower")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ShowerData), Description = "Successfully received the shower data.")]
        public async Task<HttpResponseData> GetShowerById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{UserId}/showerdata/{showerId}")] HttpRequestData req, Guid userId, Guid showerId, FunctionContext functionContext)
        {
            if (AuthCheck.CheckIfUserNotAuthorized(functionContext))
            {
                HttpResponseData responseData = req.CreateResponse();
                responseData.StatusCode = HttpStatusCode.Unauthorized;
                return responseData;
            }

            if(await userService.CheckIfUserExistAndActive(userId))
            {
                var response = showerDataService.GetShowerDataByUserId(userId, showerId);
                HttpResponseData responseData = req.CreateResponse(HttpStatusCode.OK);
                await responseData.WriteAsJsonAsync(response);
                return responseData;
            }
            else
            {
                HttpResponseData responseData = req.CreateResponse();
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }



        }


        [Function(nameof(CreateShowerDataById))]
        [OpenApiOperation(operationId: "CreateShowerDataById", tags: new[] { "Shower data" })]
        [ExampleAuth]
        [OpenApiRequestBody("application/json", typeof(CreateShowerDataDTO), Description = "Created the shower data object")]
        [OpenApiParameter(name: "UserId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The User ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(ShowerData), Description = "The OK response with the new shower.")]
        public async Task<HttpResponseData> CreateShowerDataById([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/{UserId:Guid}/showerdata")] HttpRequestData req, Guid UserId, FunctionContext functionContext)
        {
            if (AuthCheck.CheckIfUserNotAuthorized(functionContext))
            {
                HttpResponseData responseData = req.CreateResponse();
                responseData.StatusCode = HttpStatusCode.Unauthorized;
                return responseData;
            }
            _logger.LogInformation("Creating new shower.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateShowerDataDTO showerDataDTO = JsonConvert.DeserializeObject<CreateShowerDataDTO>(requestBody);



            if (await userService.CheckIfUserExistAndActive(UserId))
            {
                await showerDataService.CreateShowerDataById(showerDataDTO, UserId);
                HttpResponseData responseData = req.CreateResponse();
                responseData.StatusCode = HttpStatusCode.Created;

                return responseData;

            }
            else
            {
                HttpResponseData responseData = req.CreateResponse();
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }

        }
    }
}
