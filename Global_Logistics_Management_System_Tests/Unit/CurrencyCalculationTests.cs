using Global_Logistics_Management_System.DesignPatterns.Currency;
using Moq;

namespace Global_Logistics_Management_System_Tests.Unit
{
    public class CurrencyCalculationTests
    {
        [Fact]
        public async Task ConvertUsdToZar_WithMockedRate_ReturnsCorrectValue()
        {
            var mockStrategy = new Mock<ICurrencyConversionStrategy>();
            mockStrategy.Setup(s => s.GetExchangeRateAsync("USD", "ZAR")).ReturnsAsync(18.75m);
            var converter = new CurrencyConverter(mockStrategy.Object);

            var result = await converter.ConvertUsdToZar(100m);

            Assert.Equal(1875m, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_ZeroAmount_ReturnsZero()
        {
            var mockStrategy = new Mock<ICurrencyConversionStrategy>();
            mockStrategy.Setup(s => s.GetExchangeRateAsync("USD", "ZAR")).ReturnsAsync(18.75m);
            var converter = new CurrencyConverter(mockStrategy.Object);

            var result = await converter.ConvertUsdToZar(0m);
            Assert.Equal(0m, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_NegativeAmount_ReturnsNegative()
        {
            var mockStrategy = new Mock<ICurrencyConversionStrategy>();
            mockStrategy.Setup(s => s.GetExchangeRateAsync("USD", "ZAR")).ReturnsAsync(18.75m);
            var converter = new CurrencyConverter(mockStrategy.Object);

            var result = await converter.ConvertUsdToZar(-50m);
            Assert.Equal(-937.5m, result);
        }
    }
}
