using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Auth
{
    public interface IAuthManager
    {
        public Task<string> GetAccessKeyAsync();
    }
}
