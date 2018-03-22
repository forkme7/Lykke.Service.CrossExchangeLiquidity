using Lykke.Service.CrossExchangeLiquidity.Core.Filters.TradePart;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Tests.Filters.TradePart
{
    public class ClientIdTradePartFilterTests
    {
        private const string ClientId1 = "client id 1";
        private const string ClientId2 = "client id 2";

        private ClientIdTradePartFilter GetClientIdTradePartFilter()
        {
            return new ClientIdTradePartFilter(new ClientIdSettings() { ClientId = ClientId1 });
        }

        private Core.Domain.LykkeTrade.TradePart GetTradePart(string clientId)
        {
            return new Core.Domain.LykkeTrade.TradePart() {ClientId = clientId};
        }

        [Fact]
        public void IsAccepted_WhenClientIsRight_RetrunsTrue()
        {
            var filter = GetClientIdTradePartFilter();

            bool result = filter.IsAccepted(GetTradePart(ClientId1));

            Assert.True(result);
        }

        [Fact]
        public void IsAccepted_WhenClientIsWrong_RetrunsFalse()
        {
            var filter = GetClientIdTradePartFilter();

            bool result = filter.IsAccepted(GetTradePart(ClientId2));

            Assert.False(result);
        }
    }
}
