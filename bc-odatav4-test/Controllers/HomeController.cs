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
        #region Controller definition
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthManager _authManager;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IAuthManager authManager)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _memoryCache = memoryCache;
            _authManager = authManager;
        }
        #endregion

        #region Actions
        /// <summary>
        /// Shows all sales orders from the server.
        /// </summary>
        /// <returns>the view in Views\Home\Index.cshtml</returns>
        public async Task<IActionResult> Index(string state)
        {
            string filter;
            switch (state)
            {
                case "open":
                    filter = "&$filter=Status eq 'Open'";
                    break;
                case "released":
                    filter = "&$filter=Status eq 'Released'";
                    break;
                default: filter = ""; break;
            }

            IActionResult result = null;
            var requestUri = new Uri($"http://vsspc054:7048/BC190/ODataV4/Company('CRONUS%20Espa%C3%B1a%20S.A.')/SalesOrder?$select=No,Sell_to_Customer_No,Sell_to_Customer_Name,Status,Order_Date{filter}");
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
        /// Shows the lines from the sales order with document number passed by parameter.
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<IActionResult> Header(int no) 
        {
            IActionResult result = null;
            var requestUri = new Uri($"http://vsspc054:7048/BC190/ODataV4/Company('CRONUS%20Espa%C3%B1a%20S.A.')/workflowSalesDocumentLines?$filter=documentType%20eq%20%27Order%27%20and%20documentNumber%20eq%20%27{no}%27&$select=documentNumber,lineNumber,number,quantity,unitPrice,lineAmount");
            var response = await MakeRequestNTML(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var parser = new CustomParser<SalesOrderLine>();
                var salesLines = parser.ParseJSONValue(responseBody);

                var tableConverter = new DataTableConverter();
                var salesTable = tableConverter.ToDataTable<SalesOrderLine>(salesLines);
                result = View(salesTable);
            }
            else
            {
                result = RedirectToAction("Error");
            }
            return result;
        }

        /// <summary>
        /// Shows all the products in Sales Orders Lines, with a filter (textbox filter)
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public async Task<IActionResult> SalesLines(int numFilter)
        {
            Uri requestUri;
            if (numFilter <= 0)
                requestUri = new Uri($"http://vsspc054:7048/BC190/ODataV4/Company('CRONUS%20Espa%C3%B1a%20S.A.')/SalesLineWithStock");
            else
                requestUri = new Uri($"http://vsspc054:7048/BC190/ODataV4/Company('CRONUS%20Espa%C3%B1a%20S.A.')/SalesLineWithStock?$filter=documentNo eq '{numFilter}'");

            IActionResult result = null;
            var response = await MakeRequestNTML(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var parser = new CustomParser<SalesLineWithStock>();
                var salesLines = parser.ParseJSONValue(responseBody);

                var tableConverter = new DataTableConverter();
                var salesTable = tableConverter.ToDataTable<SalesLineWithStock>(salesLines);
                result = View(salesTable);
            }
            else
            {
                result = RedirectToAction("Error");
            }
            return result;
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
            }
            else
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
        #endregion

        #region Private methods
        /// <summary>
        /// Reads an HTTPResponseMessage to parse the "value" value. Love redundancy!
        /// </summary>
        /// <param name="responseBody"></param>
        /// <returns>a list with all json items in "value" array.</returns>
        private IEnumerable<SalesOrderHeader> ParseSalesOrders(string responseBody)
        {
            var jsonRoot = JObject.Parse(responseBody);
            var jsonValues = jsonRoot.Value<JArray>("value");
            return jsonValues.ToObject<IEnumerable<SalesOrderHeader>>();
        }

        /// <summary>
        /// Manages a request to the given endpoint using NTML authentication.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> MakeRequestNTML(Uri requestUri)
        {
            var handler = new HttpClientHandler() { Credentials = CredentialCache.DefaultCredentials, PreAuthenticate = true };
            var httpClient = new HttpClient(handler);
            return await httpClient.GetAsync(requestUri);
        }
        #endregion

    }
}
