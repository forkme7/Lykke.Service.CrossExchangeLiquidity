using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeTrade;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.TradePart;
using Lykke.Service.CrossExchangeLiquidity.Core.Services;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests.LykkeExchange
{
    public class LimitOrderMessageProcessorTests
    {
        private string OrderId = "orderId";
        private const string Asset = "BTC";
        private const decimal Volume = 1;
        private const string OppositeAsset = "ETH";
        private const decimal OppositeVolume = 2;

        private LimitOrderMessage GetLimitOrderMessage()
        {
            return new LimitOrderMessage()
            {
                Orders = new[]
                {
                    new LimitOrder()
                    {
                        Order = new Order()
                        {
                            Id = OrderId
                        },
                        Trades = new[]
                        {
                            new Trade()
                            {
                                Asset = Asset,
                                Volume = Volume,
                                OppositeAsset = OppositeAsset,
                                OppositeVolume = OppositeVolume
                            }
                        }
                    }
                }
            };
        }

        [Fact]
        public async Task Process_WhenFilterAccepts_BalanceIsChanged()
        {
            var logMock = new Mock<ILog>();
            var lykkeBalanceServiceMock = new Mock<ILykkeBalanceService>();
            var compositeExchangeMock = new Mock<ICompositeExchange>();

            var filterMock = new Mock<ITradePartFilter>();
            filterMock.Setup(f => f.IsAccepted(It.IsAny<TradePart>())).Returns(true);

            var limitOrderMessageProcessor = new LimitOrderMessageProcessor(logMock.Object,
                filterMock.Object,
                lykkeBalanceServiceMock.Object,
                compositeExchangeMock.Object);

            await limitOrderMessageProcessor.ProcessAsync(GetLimitOrderMessage());

            lykkeBalanceServiceMock.Verify(s => s.AddAssetAsync(It.IsAny<string>(), It.IsAny<decimal>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task Process_WhenFilterDoesNotAccept_BalanceIsNotChanged()
        {
            var logMock = new Mock<ILog>();
            var lykkeBalanceServiceMock = new Mock<ILykkeBalanceService>();
            var compositeExchangeMock = new Mock<ICompositeExchange>();

            var filterMock = new Mock<ITradePartFilter>();
            filterMock.Setup(f => f.IsAccepted(It.IsAny<TradePart>())).Returns(false);

            var limitOrderMessageProcessor = new LimitOrderMessageProcessor(logMock.Object,
                filterMock.Object,
                lykkeBalanceServiceMock.Object,
                compositeExchangeMock.Object);

            await limitOrderMessageProcessor.ProcessAsync(GetLimitOrderMessage());

            lykkeBalanceServiceMock.Verify(s => s.AddAssetAsync(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        }
    }
}
