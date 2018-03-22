using Common.Log;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Filters.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Services.OrderBook;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests.OrderBook
{
    public class LykkeOrderBookProcessorTests
    {
        [Fact]
        public async Task Process_WhenFilterAccepts_OrderBookIsAddedToExchange()
        {
            var lykkeExchangeMock = new Mock<ILykkeExchange>();
            var logMock = new Mock<ILog>();

            var orderBook = new LykkeOrderBook();
            var lykkeOrderBookFilterMock = new Mock<ILykkeOrderBookFilter>();
            lykkeOrderBookFilterMock.Setup(f => f.IsAccepted(orderBook)).Returns(true);
            
            var lykkeOrderBookProcessor = new LykkeOrderBookProcessor(
                logMock.Object, 
                lykkeOrderBookFilterMock.Object,
                lykkeExchangeMock.Object);
            
            await lykkeOrderBookProcessor.ProcessAsync(orderBook);

            lykkeExchangeMock.Verify(e=>e.AddOrUpdate(orderBook), Times.Once);
        }

        [Fact]
        public async Task Process_WhenFilterDoesNotAccept_OrderBookIsNotAddedToExchange()
        {            
            var logMock = new Mock<ILog>();
            var lykkeExchangeMock = new Mock<ILykkeExchange>();

            var orderBook = new LykkeOrderBook();
            var lykkeOrderBookFilterMock = new Mock<ILykkeOrderBookFilter>();
            lykkeOrderBookFilterMock.Setup(f => f.IsAccepted(orderBook)).Returns(false);
            
            var lykkeOrderBookProcessor = new LykkeOrderBookProcessor(
                logMock.Object,
                lykkeOrderBookFilterMock.Object,
                lykkeExchangeMock.Object);

            await lykkeOrderBookProcessor.ProcessAsync(orderBook);

            lykkeExchangeMock.Verify(e => e.AddOrUpdate(orderBook), Times.Never);
        }
    }
}
