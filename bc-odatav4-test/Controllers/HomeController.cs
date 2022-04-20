using bc_odatav4_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using BC;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Base URL for Business Central OData services
            var baseUrl = "http://vsspc054:7048/BC190/ODataV4/";
            //Data context
            var context = new BC.NAV(new Uri(baseUrl));

            IEnumerable<BC.Employees> showList = context.Employees.Execute();
            return View(showList);
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
