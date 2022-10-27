using Newtonsoft.Json;

namespace PrimeTime.Models
{
    internal class Output
    {
        [JsonProperty("method")]
        public string? Method { get; set; }


        [JsonProperty("prime")]
        public bool Prime { get; set; }
    }
}
