using bc_odatav4_test.Auth;
using bc_odatav4_test.Models;
using bc_odatav4_test.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace bc_odatav4_test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index()
        {
            IActionResult result = null;

            var bcHttpClient = _httpClientFactory.CreateClient("BCentral");

            bool error = false;
            string errorMessage = null;
            var authManager = new AuthManager(_httpClientFactory, _memoryCache);

            bcHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", await authManager.GetAccessKey());

            using var bcResponse = await bcHttpClient.GetAsync("Company('RRHH')/VSSImputacionesHorasRRHH");

            if (bcResponse.IsSuccessStatusCode)
            {
                var responseBody = await bcResponse.Content.ReadAsStringAsync();
                var jsonRoot = JObject.Parse(responseBody);
                var jsonValues = jsonRoot.Value<JArray>("value");
                var hourInputs = jsonValues.ToObject<IEnumerable<HourInput>>();

                var tableConverter = new DataTableConverter();
                var hourInputsTable = tableConverter.ToDataTable<HourInput>(hourInputs);
                result = View(hourInputsTable);
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
