using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Models
{
    public class SalesOrderHeaders
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
        public string Bill_to_Customer_No { get; set; }
        public string Status { get; set; }
        public string Person_Responsible { get; set; }
        public string Search_Description { get; set; }
        public string Project_Manager { get; set; }
    }
}
