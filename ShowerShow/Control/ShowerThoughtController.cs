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
using ShowerShow.Models;
using ShowerShow.DTO;
using ShowerShow.Repository.Interface;
using System.Collections.Generic;
using System.Collections;
using ShowerShow.Model;
using ShowerShow.Service;
using System.Configuration;

namespace ShowerShow.Controllers
{
    public class ShowerThoughtController
    {
        private readonly ILogger<ShowerThoughtController> _logger;
        private IShowerThoughtService showerThoughtService;
        private IUserService userService;

        public ShowerThoughtController(ILogger<ShowerThoughtController> log, IShowerThoughtService showerThoughtService, IUserService userService)
        {
            _logger = log;
            this.showerThoughtService = showerThoughtService;
            this.userService = userService;
        }
        [Function("CreateShowerThought")]
        [OpenApiOperation(operationId: "CreateShowerThought", tags: new[] { "ShowerThoughts" })]
        [OpenApiRequestBody("application/json", typeof(ShowerThoughtDTO), Description = "The shower thought data.")]
        [OpenApiParameter(name: "ShowerId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ShowerId parameter")]
        [OpenApiParameter(name: "UserId", In = ParameterLocation.Query, Required = false, Type = typeof(Guid), Description = "The User ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(ShowerThought), Description = "The OK response with the new thought.")]
        public async Task<HttpResponseData> CreateShowerThought([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "shower/thoughts/{ShowerId:Guid}")] HttpRequestData req, Guid ShowerId, Guid UserId)
        {
            _logger.LogInformation("Creating new thought.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            HttpResponseData responseData = req.CreateResponse();

            if (await userService.CheckIfUserExist(UserId))
            {
               // passing both shower id and user id because I dont have a ShowerData class
               // later get UserId based on ShowerId
                ShowerThoughtDTO data = JsonConvert.DeserializeObject<ShowerThoughtDTO>(requestBody);
                await showerThoughtService.CreateShowerThought(data,ShowerId,UserId);
                await responseData.WriteAsJsonAsync(data);
                responseData.StatusCode = HttpStatusCode.Created;
                return responseData;
            }
            else
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }
        }
        [Function("DeleteShowerThought")]
        [OpenApiOperation(operationId: "DeleteShowerThought", tags: new[] { "ShowerThoughts" })]
        [OpenApiParameter(name: "ThoughtId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ShowerThought ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ShowerThought), Description = "The OK response with the deleted thought")]
        public async Task<HttpResponseData> DeleteShowerThought([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "shower/thoughts/{ThoughtId:Guid}")] HttpRequestData req, Guid ThoughtId)
        {
            _logger.LogInformation("Deleting thought.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            HttpResponseData responseData = req.CreateResponse();
            try
            {
                ShowerThought thought = null;

                //this is to give priority to tasks
                Task getId = Task.Run(() =>
                {
                    thought = showerThoughtService.GetShowerThoughtById(ThoughtId).Result;
                });
                await getId.ContinueWith(prev =>
                {
                    showerThoughtService.DeleteShowerThought(thought);
                });
                responseData.StatusCode = HttpStatusCode.OK;
                await responseData.WriteAsJsonAsync(thought);
                return responseData;

            }
            catch
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }
        }
        [Function("GetThoughtById")]
        [OpenApiOperation(operationId: "GetThoughtById", tags: new[] { "ShowerThoughts" })]
        [OpenApiParameter(name: "ThoughtId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The thought ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ShowerThought), Description = "The OK response with the retrieved thought")]
        public async Task<HttpResponseData> GetThoughtById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shower/thoughts/{ThoughtId:Guid}")] HttpRequestData req, Guid ThoughtId)
        {
            _logger.LogInformation("Retrieving schedule.");


            HttpResponseData responseData = req.CreateResponse();
            try
            {
                ShowerThought thought = await showerThoughtService.GetShowerThoughtById(ThoughtId);
                await responseData.WriteAsJsonAsync(thought);
                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;

            }
            catch
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }
        }
        [Function("GetThoughtByShowerId")]
        [OpenApiOperation(operationId: "GetThoughtByShowerId", tags: new[] { "ShowerThoughts" })]
        [OpenApiParameter(name: "ShowerId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The shower ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ShowerThought), Description = "The OK response with the retrieved thoughts")]
        public async Task<HttpResponseData> GetThoughtByShowerId([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shower/thoughts/{ShowerId:Guid}/s")] HttpRequestData req, Guid ShowerId)
        {
            _logger.LogInformation("Retrieving schedule.");


            HttpResponseData responseData = req.CreateResponse();
            try
            {
                ShowerThought thought = await showerThoughtService.GetThoughtByShowerId(ShowerId);
                await responseData.WriteAsJsonAsync(thought);
                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;

            }
            catch
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }
        }
        [Function("GetThoughtsByUserId")]
        [OpenApiOperation(operationId: "GetThoughtsByUserId", tags: new[] { "ShowerThoughts" })]
        [OpenApiParameter(name: "UserId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The user ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ShowerThought), Description = "The OK response with the retrieved thoughts")]
        public async Task<HttpResponseData> GetThoughtsByUserId([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shower/thoughts/{UserId:Guid}/u")] HttpRequestData req, Guid UserId)
        {
            _logger.LogInformation("Retrieving thoughts.");


            HttpResponseData responseData = req.CreateResponse();
            if(await userService.CheckIfUserExist(UserId))
            {
                List<ShowerThought> thoughts = (List<ShowerThought>)await showerThoughtService.GetAllShowerThoughtsForUser(UserId);
                await responseData.WriteAsJsonAsync(thoughts);
                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;

            }
            else
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }
        }
        [Function("GetThoughtsByDate")]
        [OpenApiOperation(operationId: "GetThoughtsByDate", tags: new[] { "ShowerThoughts" })]
        [OpenApiParameter(name: "Date", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The date parameter")]
        [OpenApiParameter(name: "UserId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The user ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ShowerThought), Description = "The OK response with the retrieved thoughts")]
        public async Task<HttpResponseData> GetThoughtsByDate([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shower/thoughts/{UserId:Guid}/date")] HttpRequestData req, Guid UserId, string Date)
        {
            _logger.LogInformation("Retrieving thoughts.");


            HttpResponseData responseData = req.CreateResponse();
            if(await userService.CheckIfUserExist(UserId))
            {
                DateTime dateTime = DateTime.ParseExact(Date, "dd-MM-yyyy",System.Globalization.CultureInfo.InvariantCulture);
                List<ShowerThought> thoughts = (List<ShowerThought>)await showerThoughtService.GetShowerThoughtsByDate(dateTime, UserId);
                await responseData.WriteAsJsonAsync(thoughts);
                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;

            }
            else
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                return responseData;
            }
        }
    }
}

