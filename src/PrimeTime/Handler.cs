using Newtonsoft.Json;
using PrimeTime.Models;
using System.Numerics;

namespace PrimeTime
{
    internal class Handler
    {
        public Handler() { }

        public async Task ProcessRequestsAsync(StreamReader reader, StreamWriter writer)
        {
            string? json;
            writer.AutoFlush = true;
            while ((json = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
            {
                try
                {
                    bool prime = ProcessJsonAndValidate(json);
                    Output output = new Output
                    {
                        Method = "isPrime",
                        Prime = prime,
                    };
                    string jsonOutput = JsonConvert.SerializeObject(output);
                    await writer.WriteAsync($"{jsonOutput}\n").ConfigureAwait(false);
                    Console.WriteLine($"Input: {json} | Output: {jsonOutput}");
                }
                catch (JsonException)
                {
                    await writer.WriteAsync("{\"error\":true}\n").ConfigureAwait(false);
                    break;
                }
            }
        }

        private bool ProcessJsonAndValidate(string? json)
        {
            if (json == null) {
                return false;
            }
            Input? input = JsonConvert.DeserializeObject<Input>(json);
            if (input == null || input.Method != "isPrime")
            {
                throw new JsonException();
            }
            bool prime = false;
            if (Math.Abs(input.Number % 1) <= (Double.Epsilon * 100) || input.Number < 0)
            {
                prime = IsPrime((BigInteger)input.Number);
            }
            return prime;
        }

        private bool IsPrime(BigInteger number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            for (BigInteger i = 3; BigInteger.Pow(i, 2) <= number; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }
    }
}
