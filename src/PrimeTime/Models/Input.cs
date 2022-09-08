using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PrimeTime.Models
{
    internal class Input
    {
        [JsonProperty("method", Required = Required.Always)]
        public string Method { get; set; }


        [JsonProperty("number", Required = Required.Always)]
        [JsonConverter(typeof(StrictIntConverter))]
        public double Number { get; set; }

        // everything else gets stored here
        [JsonExtensionData]
        private IDictionary<string, JToken>? _additionalData;
    }
}
