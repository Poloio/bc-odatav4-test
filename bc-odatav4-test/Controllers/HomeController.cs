using bc_odatav4_test.Auth;
using bc_odatav4_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

            var bcHttpClient = _httpClientFactory.CreateClient("BCentral");
            var authManager = new AuthManager();

            bool error = false;
            string errorMessage = null;

            bcHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", compKeyResponse.token_type + " " + compKeyResponse.access_token);

            using var bcResponse = await bcHttpClient.GetAsync("Company('RRHH')/VSSImputacionesHorasRRHH");

            if (bcResponse.IsSuccessStatusCode)
            {
                using var bcContentStream =
                await bcResponse.Content.ReadAsStreamAsync();

                dynamic hourInputs = await JsonConvert.(bcResponse);
            } else
            {
                error = true;
                errorMessage = bcResponse.StatusCode + " - " + bcResponse.ReasonPhrase;
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
