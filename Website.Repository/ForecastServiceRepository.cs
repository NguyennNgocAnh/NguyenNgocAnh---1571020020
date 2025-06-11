using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Website.Repository
{
    public class ForecastServiceRepository
    {
        private readonly HttpClient _httpClient;

        public ForecastServiceRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<float?> GetPredictionAsync(float[] data)
        {
            var requestBody = new { data = data };

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/predict", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadFromJsonAsync<PredictionResponse>();
                return jsonResult?.Prediction;
            }

            // Xử lý lỗi hoặc trả về null
            return null;
        }
    }

    public class PredictionResponse
    {
        public float Prediction { get; set; }
    }

}
