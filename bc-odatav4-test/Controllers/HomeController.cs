using bc_odatav4_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace bc_odatav4_test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IActionResult result = null;

            var oAuthHttpClient = _httpClientFactory.CreateClient("OAuth");
            var bcHttpClient = _httpClientFactory.CreateClient("BCentral");

            // Prepare body for the access key request
            var keyRequestBody = new StringContent(
               // body
               "grant_type=client_credentials&client_id=83200fbd-baec-404e-aac5-16611b8c7e9b&client_secret=S8O7Q~slAX7S4FwodwlF4ZgoHTqfDZCfac73C&scope=openid https://api.businesscentral.dynamics.com/.default offline_access",
               Encoding.UTF8,
               // content-type header
               "application/x-www-form-urlencoded");

            using var keyResponse = await oAuthHttpClient.PostAsync("53b34312-3c82-4e55-ad41-dc58497e9bd8/oauth2/v2.0/token", keyRequestBody);

            var error = false;
            var errorMessage = "";
            if (keyResponse.IsSuccessStatusCode)
            {
                using var contentStream =
                    await keyResponse.Content.ReadAsStreamAsync();
                
                var compKeyResponse = await JsonSerializer.DeserializeAsync
                    <OAuthToken>(contentStream);

                if (compKeyResponse.token_type != null & compKeyResponse.access_token != null)
                    bcHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", compKeyResponse.token_type + " " + compKeyResponse.access_token);

                using var bcResponse = await bcHttpClient.GetAsync("Company('RRHH')/VSSImputacionesHorasRRHH");

                if (bcResponse.IsSuccessStatusCode)
                {
                    using var bcContentStream =
                    await bcResponse.Content.ReadAsStreamAsync();

                    /* I MISS JAVASCRIPT FOR THIS
                    var hourInputs = await JsonSerializer.DeserializeAsync
                        <IEnumerable<Object>>(bcContentStream); doesn't work, but I need OData unchased anyways
                    */

                    result = View(bcResponse.Content.ToString());
                } else
                {
                    error = true;
                    errorMessage = bcResponse.StatusCode + " - " + bcResponse.ReasonPhrase;
                }
            } else
            {
                error = true;
            }

            if (error)
            {
                result = RedirectToAction("Error");
            }

            return result;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
