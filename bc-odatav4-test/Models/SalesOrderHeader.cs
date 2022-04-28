using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Models
{
    public class SalesOrderHeader
    {
        public string no { get; set; }
        public string sellToCustomerNo { get; set; }
        public string sellToCustomerName { get; set; }
        public string orderDate { get; set; }
        public string status { get; set; }
        public int totalItems { get; set; }
    }
}
