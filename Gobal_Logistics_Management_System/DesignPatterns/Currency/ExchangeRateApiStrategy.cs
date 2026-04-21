using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Global_Logistics_Management_System.DesignPatterns.Currency
{
    public class ExchangeRateApiStrategy : ICurrencyConversionStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExchangeRateApiStrategy> _logger;

        public ExchangeRateApiStrategy(HttpClient httpClient, ILogger<ExchangeRateApiStrategy> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
        {
            try
            {
                var url = $"latest/{baseCurrency}";
                var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(url);
                if (response?.Rates != null && response.Rates.TryGetValue(targetCurrency, out decimal rate))
                    return rate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExchangeRate API strategy failed.");
                throw;
            }
            throw new InvalidOperationException("Unable to retrieve exchange rate.");
        }

        private class ExchangeRateResponse
        {
            [JsonPropertyName("rates")]
            public Dictionary<string, decimal> Rates { get; set; } = new();
        }
    }
}
