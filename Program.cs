using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RainfallApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiClient = new RainfallApiClient();

            // Blackpool station ID - obtained from the example in documentation
            var stationId = "577271";

            try
            {
                var response = await apiClient.GetRainfallReadingsAsync(stationId);

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

        public async Task<RainfallReadingResponse> GetRainfallReadingsAsync(string stationId, int count = 10)
        {

            // Constructing the request URL
            // - See API Summary Table in documentation:
            //    'All readings for measures from a particular station'
            var url = $"{BaseUrl}/id/stations/{stationId}/readings";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get rainfall readings. Status code: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RainfallReadingResponse>(content);
        }
    }

    // Define the data contract for rainfall readings
    public class RainfallReading
    {
        [JsonProperty("@id")] 
        public string Id { get; set; }

        [JsonProperty("dateTime")]
        public DateTime DateMeasured { get; set; }

        [JsonProperty("measure")] 
        public string Measure { get; set; }

        [JsonProperty("value")]
        public decimal AmountMeasured { get; set; }
    }
    public class RainfallReadingResponse
    {
        public List<RainfallReading> Items { get; set; }
    }
}
