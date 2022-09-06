using WebAuth.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Security.Claims;

namespace WebAuth.Services
{
    public interface IApiHelper
    {
        Task<string> Authenticate(string userName, string password);
        Task LogOffUser();
        Task<bool> IsValidToken(string token);
        Task<ClaimsPrincipal> GetUser();

    }
    public class ApiHelper : IApiHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public ApiHelper(IHttpClientFactory httpClientFactory, string baseUrl = "authapi")
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = baseUrl;
        }

        public async Task<string> Authenticate(string username, string password)
        {
            var httpClient = _httpClientFactory.CreateClient(_baseUrl);
            var data = new StringContent(JsonSerializer.Serialize(new UserLoginModel
            {

                UserName = username,
                Password = password
            }),
            Encoding.UTF8, "application/json");

            using HttpResponseMessage response = await httpClient.PostAsync("/api/Auth/login", data);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthenticatedUser>();
                return result?.Token ?? "";
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        // TODO: This does not work, returns 500 error
        public async Task<ClaimsPrincipal> GetUser()
        {
            var httpClient = _httpClientFactory.CreateClient(_baseUrl);        

            using HttpResponseMessage response = await httpClient.GetAsync("/api/Auth/user");
            if (response.IsSuccessStatusCode)
            {                
                var result = await response.Content.ReadFromJsonAsync<ClaimsPrincipal>();                
                return result;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<bool> IsValidToken(string token)
        {
            var httpClient = _httpClientFactory.CreateClient(_baseUrl);
            using HttpResponseMessage response = await httpClient.GetAsync("/api/Auth/validate");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<bool>();
                return result;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public Task LogOffUser()
        {
            throw new NotImplementedException();
        }
    }
}
