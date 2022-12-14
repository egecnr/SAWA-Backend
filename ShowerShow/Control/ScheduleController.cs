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

namespace ShowerShow.Controllers
{
    public class ScheduleController
    {
        private readonly ILogger<ScheduleController> _logger;
        private IScheduleService scheduleService;

        public ScheduleController(ILogger<ScheduleController> log, IScheduleService scheduleService)
        {
            _logger = log;
            this.scheduleService = scheduleService;
        }
        [Function("CreateSchedule")]
        [OpenApiOperation(operationId: "CreateSchedule", tags: new[] { "Schedules" })]
        [OpenApiRequestBody("application/json", typeof(CreateScheduleDTO), Description = "The schedule data.")]
        [OpenApiParameter(name: "UserId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The User ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Schedule), Description = "The OK response with the new schedule.")]
        public async Task<HttpResponseData> CreateSchedule([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "schedule/{UserId:Guid}")] HttpRequestData req, Guid UserId)
        {
            _logger.LogInformation("Creating new schedule.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            HttpResponseData responseData = req.CreateResponse();
            try
            {

                CreateScheduleDTO data = JsonConvert.DeserializeObject<CreateScheduleDTO>(requestBody);
                await scheduleService.AddScheduleToQueue(data, UserId);
                await responseData.WriteAsJsonAsync(data);
                responseData.StatusCode = HttpStatusCode.Created;
                return responseData;
            }
            catch (Exception ex)
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                responseData.Headers.Add("Reason", ex.Message);
                return responseData;

            }
        }
        [Function("GetSchedules")]
        [OpenApiOperation(operationId: "GetSchedules", tags: new[] { "Schedules" })]
        [OpenApiParameter(name: "UserId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The User ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<Schedule>), Description = "Retrieved schedules")]
        public async Task<HttpResponseData> GetSchedules([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "schedule/{UserId:Guid}/schedules")] HttpRequestData req, Guid UserId)
        {
            _logger.LogInformation("Getting schedules.");
            HttpResponseData responseData = req.CreateResponse();

            try
            {

                List<Schedule> schedules = (List<Schedule>)await scheduleService.GetAllSchedules(UserId);
                await responseData.WriteAsJsonAsync(schedules);

                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;

            }
            catch (Exception ex)
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                responseData.Headers.Add("Reason", ex.Message);
                return responseData;
            }
        }
        [Function("GetScheduleById")]
        [OpenApiOperation(operationId: "GetScheduleById", tags: new[] { "Schedules" })]
        [OpenApiParameter(name: "ScheduleId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The Schedule ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Schedule), Description = "The OK response with the retrieved schedule")]
        public async Task<HttpResponseData> GetScheduleById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "schedule/{ScheduleId:Guid}")] HttpRequestData req, Guid ScheduleId)
        {
            _logger.LogInformation("Retrieving schedule.");


            HttpResponseData responseData = req.CreateResponse();
            try
            {
                Schedule schedule = await scheduleService.GetScheduleById(ScheduleId);
                await responseData.WriteAsJsonAsync(schedule);
                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;

            }
            catch(Exception ex)
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                responseData.Headers.Add("Reason", ex.Message);
                return responseData;
            }
        }
        [Function("DeleteSchedule")]
        [OpenApiOperation(operationId: "DeleteScheduleById", tags: new[] { "Schedules" })]
        [OpenApiParameter(name: "ScheduleId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The Schedule ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Schedule), Description = "The OK response with the deleted schedule")]
        public async Task<HttpResponseData> DeleteSchedule([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "schedule/{ScheduleId:Guid}")] HttpRequestData req, Guid ScheduleId)
        {
            _logger.LogInformation("Deleting schedule.");
            HttpResponseData responseData = req.CreateResponse();
            try
            { 
                await scheduleService.DeleteSchedule(ScheduleId);
                responseData.StatusCode = HttpStatusCode.OK;
                return responseData;

            }
            catch (Exception ex)
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                responseData.Headers.Add("Reason", ex.Message);
                return responseData;
            }
        }
        [Function("UpdateSchedule")]
        [OpenApiOperation(operationId: "UpdateSchedule", tags: new[] { "Schedules" })]
        [OpenApiRequestBody("application/json", typeof(UpdateScheduleDTO), Description = "The schedule data.")]
        [OpenApiParameter(name: "ScheduleId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The schedule ID parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(Schedule), Description = "The OK response with the updated schedule.")]
        public async Task<HttpResponseData> UpdateSchedule([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "schedule/{ScheduleId:Guid}")] HttpRequestData req, Guid ScheduleId)
        {
            _logger.LogInformation("Updating.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            HttpResponseData responseData = req.CreateResponse();
            try
            {
                UpdateScheduleDTO newSchedule = JsonConvert.DeserializeObject<UpdateScheduleDTO>(requestBody);

                Schedule schedule = await scheduleService.UpdateSchedule(ScheduleId, newSchedule);
                responseData.StatusCode = HttpStatusCode.OK;
                await responseData.WriteAsJsonAsync(schedule);
                return responseData;
            }
            catch (Exception ex)
            {
                responseData.StatusCode = HttpStatusCode.BadRequest;
                responseData.Headers.Add("Reason", ex.Message);
                return responseData;
            }
        }
    }
}

