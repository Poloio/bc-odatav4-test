using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace bc_odatav4_test.Auth
{
    public class AuthManager
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        public AuthManager(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }
        public async Task<string> GetAccessKey()
        {
            var returnString = "";

            if (!_memoryCache.TryGetValue("AccessToken", out DateTime cacheValue))
            { 
                var client = _httpClientFactory.CreateClient("OAuth");
                // Prepare body for the access key request
                var requestBody = new StringContent(
                   // body
                   "grant_type=client_credentials&client_id=83200fbd-baec-404e-aac5-16611b8c7e9b&client_secret=S8O7Q~slAX7S4FwodwlF4ZgoHTqfDZCfac73C&scope=openid https://api.businesscentral.dynamics.com/.default offline_access",
                   Encoding.UTF8,
                   // content-type header
                   "application/x-www-form-urlencoded");


                using var response = await client.PostAsync("53b34312-3c82-4e55-ad41-dc58497e9bd8/oauth2/v2.0/token", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream =
                        await response.Content.ReadAsStreamAsync();

                    var compKeyResponse = await JsonSerializer.DeserializeAsync
                        <OAuthToken>(contentStream);

                    returnString = compKeyResponse.token_type + " " + compKeyResponse.access_token;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(1));

                _memoryCache.Set("AccessToken", returnString, cacheEntryOptions);
            } 
            else
            {
                returnString = _memoryCache.Get("AccessToken").ToString();
            }

            return returnString;
        }
    }
}
