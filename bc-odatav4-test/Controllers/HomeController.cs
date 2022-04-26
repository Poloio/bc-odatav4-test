using bc_odatav4_test.Auth;
using bc_odatav4_test.Models;
using bc_odatav4_test.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
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
        /// <summary>
        /// Shows all sales orders from the server.
        /// </summary>
        /// <returns>the view in Views\Home\Index.cshtml</returns>
        public async Task<IActionResult> Index()
        {
            IActionResult result = null;
            var requestUri = new Uri("http://vsspc054:7048/BC190/ODataV4/Company('CRONUS%20Espa%C3%B1a%20S.A.')/SalesOrder");
            var response = await MakeRequestNTML(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var responseBody =  await response.Content.ReadAsStringAsync();
                var salesOrders = ParseSalesOrders(responseBody);

                var tableConverter = new DataTableConverter();
                var salesTable = tableConverter.ToDataTable<SalesOrderHeader>(salesOrders);
                result = View(salesTable);
            }
            else
            {
                result = RedirectToAction("Error");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        private IEnumerable<SalesOrderHeader> ParseSalesOrders(string responseBody)
        {
            var jsonRoot = JObject.Parse(responseBody);
            var jsonValues = jsonRoot.Value<JArray>("value");
            return jsonValues.ToObject<IEnumerable<SalesOrderHeader>>();
        }

        private async Task<HttpResponseMessage> MakeRequestNTML(Uri requestUri)
        {
            var handler = new HttpClientHandler() { Credentials = CredentialCache.DefaultCredentials, PreAuthenticate = true };
            var httpClient = new HttpClient(handler);
            return await httpClient.GetAsync(requestUri);
        }

        public async Task<IActionResult> OnPrem()
        {
            IActionResult result = null;

            bool error = false;
            string errorMessage = null;

            var handler = new HttpClientHandler() { Credentials = CredentialCache.DefaultCredentials, PreAuthenticate = true };
            var httpClient = new HttpClient(handler);

            var completeUri = new Uri("http://vsspc054:7048/BC190/ODataV4/Company('CRONUS%20Espa%C3%B1a%20S.A.')/Job_List?$select=No,Description,Bill_to_Customer_No,Status,Person_Responsible,Search_Description,Project_Manager");
            using var bcResponse = await httpClient.GetAsync(completeUri);

            if (bcResponse.IsSuccessStatusCode)
            {
                var responseBody = await bcResponse.Content.ReadAsStringAsync();
                var jsonRoot = JObject.Parse(responseBody);
                var jsonValues = jsonRoot.Value<JArray>("value");
                var projects = jsonValues.ToObject<IEnumerable<SalesOrderHeaders>>();

                var tableConverter = new DataTableConverter();
                var hourInputsTable = tableConverter.ToDataTable<SalesOrderHeaders>(projects);
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
