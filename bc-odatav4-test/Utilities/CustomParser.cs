using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bc_odatav4_test.Utilities
{
    public class CustomParser<T>
    {
        /// <summary>
        /// Reads an HTTPResponseMessage to parse the "value" value. Love redundancy!
        /// </summary>
        /// <param name="responseBody"></param>
        /// <returns>a list with all json items in "value" array.</returns>
        public IEnumerable<T> ParseJSONValue(string responseBody)
        {
            var jsonRoot = JObject.Parse(responseBody);
            var jsonValues = jsonRoot.Value<JArray>("value");
            return jsonValues.ToObject<IEnumerable<T>>();
        }
    }
}
