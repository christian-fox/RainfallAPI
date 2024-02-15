using Newtonsoft.Json;
using System.Text.Json;

namespace RainfallApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiClient = new RainfallApiClient();

            // Blackpool station ID - obtained from the example in documentation
            var stationId = "577271";

            Console.Write("Enter the number of responses (count) to get: ");
            string? countStr = Console.ReadLine();
            int count;
            if (!int.TryParse(countStr, out count))
            {
                // If userInputtedString was not castable to 'int' type, default to 10
                count = 10;
            }

            try
            {
                var response = await apiClient.GetRainfallReadingsAsync(stationId, count);

                Console.WriteLine($"Rainfall readings for station ID {stationId}:");
                foreach (var reading in response.Items)
                {
                    Console.WriteLine($"Date: {reading.DateMeasured}, Amount: {reading.AmountMeasured}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    public class RainfallApiClient
    {
        private readonly HttpClient _httpClient;
        // root
        private const string BaseUrl = "http://environment.data.gov.uk/flood-monitoring";

        public RainfallApiClient()
        {
            _httpClient = new HttpClient();
        }

        // 'get-rainfall' in the spec
        public async Task<RainfallReadingResponse> GetRainfallReadingsAsync(string stationId, int userSpecifiedCount = 10)
        {
            // Testing the bounds: 1 <= count <= 100
            int count = userSpecifiedCount;
            if (userSpecifiedCount < 1)
            {
                Console.WriteLine("Minimum count is 1");
                count = 1;
            }
            else if (userSpecifiedCount > 100)
            {
                Console.WriteLine("Maximum count is 100");
                count = 100;
            }

            // Constructing the request URL
            // - See API Summary Table in documentation:
            //    'All readings for measures from a particular station'
            var url = $"{BaseUrl}/id/stations/{stationId}/readings?_limit={count}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get rainfall readings. Status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            // provide a standard new response if 'content' is null
            return JsonConvert.DeserializeObject<RainfallReadingResponse>(content) ?? new RainfallReadingResponse(); ;
        }
    }

    // Define the data contract for rainfall readings
    public class RainfallReading
    {
        [JsonProperty("dateTime")]
        public DateTime DateMeasured { get; set; }

        [JsonProperty("value")]
        public decimal AmountMeasured { get; set; }
    }
    public class RainfallReadingResponse
    {
        public List<RainfallReading> Items { get; set; } = new List<RainfallReading>();
    }
}
