using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Models
{
    public class HourInput
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public int id { get; set; }
        public string empresa { get; set; }
        public string obra { get; set; }
        public string personal_id { get; set; }
        public string fecha { get; set; }
        public int num_horas { get; set; }
        public int coste_hora { get; set; }
        public DateTime Processing_datetime { get; set; }
        public string Tarea { get; set; }
        public string personal_nombre { get; set; }
        public string personal_apellidos { get; set; }
    }
}
