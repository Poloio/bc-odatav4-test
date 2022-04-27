using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Models
{
    public class SalesLineWithStock
    {
        public string documentNo { get; set; }
        public int lineNo { get; set; }
        public string no { get; set; }
        public int quantity { get; set; }
        public int unitsInStock { get; set; }
    }
}
