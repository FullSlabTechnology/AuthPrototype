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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace WebAuth.Services
{
    public interface IApiHelper
    {
        Task<string> Authenticate(string userName, string password);
        Task LogOffUser();
        Task<bool> IsValidToken(string token);
        Task<AuthenticatedUser> GetUser();

    }
    public class ApiHelper : IApiHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;

        public ApiHelper(IHttpClientFactory httpClientFactory, IConfiguration config, string baseUrl = "authapi")
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
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
        public async Task<AuthenticatedUser> GetUser()
        {
            var httpClient = _httpClientFactory.CreateClient(_baseUrl);

            using HttpResponseMessage response = await httpClient.GetAsync("/api/Auth/user");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthenticatedUser>();
                return result;
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<bool> IsValidToken(string token)
        {
            //var httpClient = _httpClientFactory.CreateClient(_baseUrl);
            //using HttpResponseMessage response = await httpClient.GetAsync("/api/Auth/validate");
            //if (response.IsSuccessStatusCode)
            //{
            //    var result = await response.Content.ReadFromJsonAsync<bool>();
            //    return result;
            //}
            //else
            //{
            //    throw new Exception(response.ReasonPhrase);
            //}
            return IsTokenValid(
                key: _config.GetSection("Jwt")["Key"],
                issuer: _config.GetSection("Jwt")["Issuer"],
                audience: _config.GetSection("Jwt")["Audience"],
                token: token);                       
        }

        public bool IsTokenValid(string key, string issuer, string audience, string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }


        public Task LogOffUser()
        {
            throw new NotImplementedException();
        }
    }
}
