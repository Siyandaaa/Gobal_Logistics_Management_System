using System.Text.Json.Serialization;

namespace Global_Logistics_Management_System.Services
{
    public interface ICurrencyService
    {
        Task<decimal> GetUsdToZarRateAsync();
        decimal ConvertUsdToZar(decimal usdAmount, decimal rate);
    }

    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(HttpClient httpClient, IConfiguration config, ILogger<CurrencyService> logger)
        {
            _httpClient = httpClient;
            _config = config;
            _logger = logger;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>("latest/USD");
                if (response?.Rates != null && response.Rates.TryGetValue("ZAR", out decimal rate))
                    return rate;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Currency API request failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during currency conversion.");
            }

            // Fallback rate from config
            var fallback = _config.GetValue<decimal>("CurrencyApi:FallbackRate", 18.5m);
            _logger.LogWarning("Using fallback exchange rate: {Rate}", fallback);
            return fallback;
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal rate) => usdAmount * rate;
    }

    // DTO for API response
    public class ExchangeRateResponse
    {
        [JsonPropertyName("rates")]
        public Dictionary<string, decimal>? Rates { get; set; }
        [JsonPropertyName("base")]
        public string? Base { get; set; }
        [JsonPropertyName("date")]
        public string? Date { get; set; }
    }
}
