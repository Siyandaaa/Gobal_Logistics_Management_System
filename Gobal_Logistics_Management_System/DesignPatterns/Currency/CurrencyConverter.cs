namespace Global_Logistics_Management_System.DesignPatterns.Currency
{
    public class CurrencyConverter
    {
        private readonly ICurrencyConversionStrategy _strategy;
        public CurrencyConverter(ICurrencyConversionStrategy strategy) => _strategy = strategy;

        public async Task<decimal> ConvertUsdToZar(decimal usdAmount)
        {
            var rate = await _strategy.GetExchangeRateAsync("USD", "ZAR");
            return usdAmount * rate;
        }
    }
}
