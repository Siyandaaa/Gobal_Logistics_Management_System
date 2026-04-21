using Global_Logistics_Management_System.Services;

namespace Global_Logistics_Management_System.DesignPatterns.Currency
{
    public interface ICurrencyConversionStrategy
    {
        Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
    }
}
