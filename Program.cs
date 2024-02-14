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

            // Replace "YOUR_STATION_ID" with the desired station ID
            var stationId = "577271";

            try
            {
                var response = await apiClient.GetRainfallReadingsAsync(stationId);

                Console.WriteLine($"Rainfall readings for station ID {stationId}:");
                foreach (var reading in response.Readings)
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
        private const string BaseUrl = "https://environment.data.gov.uk/flood-monitoring/id/";

        public RainfallApiClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<RainfallReadingResponse> GetRainfallReadingsAsync(string stationId, int count = 10)
        {
            var url = $"{BaseUrl}{stationId}/readings?count={count}";

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
        public DateTime DateMeasured { get; set; }
        public decimal AmountMeasured { get; set; }
    }
    public class RainfallReadingResponse
    {
        public List<RainfallReading> Readings { get; set; }
    }
}
