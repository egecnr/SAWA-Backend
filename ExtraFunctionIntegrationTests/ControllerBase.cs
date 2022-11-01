﻿using ExtraFunction.Model;
using Newtonsoft.Json;
using System.Text;
using Xunit.Abstractions;

namespace ExtraFunctionIntegrationTest
{
    public class LoginResultDTO
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
    public class ControllerBase
    {
        protected HttpClient client { get; }
        protected ITestOutputHelper outputHelper;
        public ControllerBase(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
            this.client = new HttpClient() {
                BaseAddress = new Uri($"http://localhost:7071/api/")
                //http://localhost:7071/api/Login"
            };
        }

        protected async Task Authenticate()
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetAuthString());
        }

        private async Task<string> GetAuthString()
        {
            Login loginUser = new Login() {Username="test",Password="test"};
            string requesturi = "Login";
            HttpContent http = new StringContent(JsonConvert.SerializeObject(loginUser),Encoding.UTF8,"application/json");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = client.PostAsync(requesturi, http).Result;

            var authString = (await response.Content.ReadAsAsync<LoginResultDTO>()).AccessToken;
            return authString;
        }

        public async Task FlushUser(string username)
        {
            string requestUri = $"user/{username}";
            await Authenticate();
            var response = await client.DeleteAsync(requestUri);
        }
    }
}