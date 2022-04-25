using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Auth
{
    public class OAuthToken
    {
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public int ext_expires_in { get; set; }
        public string access_token { get;set; }
    }
}
