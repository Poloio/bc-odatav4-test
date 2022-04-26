using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Models
{
    public class SalesOrderLine
    {
        public string documentNumber { get; set; }
        public int lineNumber { get; set; }
        public string number { get; set; }
        public int quantity { get; set; }
        public double unitPrice { get; set; }
    }
}
