using bc_odatav4_test.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bc_odatav4_test.Controllers
{
    public class CrudController : Controller
    {
        /// <summary>
        /// Shows a menu to select an action.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            IActionResult result = null;
            var endpointUri = new Uri("http://vsspc054:7048/BC190/ODataV4/ItemCrud_Create?company='CRONUS%20Espa%C3%B1a%20S.A.'");
            var jsonBody = JsonConvert.SerializeObject(product);
            var httpResponse = await MakePostRequestNTML(endpointUri, jsonBody);
            if (httpResponse.IsSuccessStatusCode)
            {
                result = RedirectToAction("Index");
            } else
            {
                result = View(product);
            }
            return result;
        }

        /// <summary>
        /// Manages a request to the given endpoint using NTML authentication.
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> MakePostRequestNTML(Uri requestUri, string body)
        {
            var handler = new HttpClientHandler() { Credentials = CredentialCache.DefaultCredentials, PreAuthenticate = true };
            var httpClient = new HttpClient(handler);
            var requestMessage = new HttpRequestMessage();

            requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
            requestMessage.RequestUri = requestUri;
            requestMessage.Method = HttpMethod.Post;

            return await httpClient.SendAsync(requestMessage);
        }
    }
}
