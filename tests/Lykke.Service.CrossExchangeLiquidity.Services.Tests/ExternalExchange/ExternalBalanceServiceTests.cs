using System;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalBalance;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.ExternalExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.ExternalExchange;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests.ExternalExchange
{
    public class ExternalBalanceServiceTests
    {
        private const decimal Balance = 10;
        private const decimal DefaultBalance = 10;
        private const string AssetId = "BTC";
        private const string Source = "bitfinex";
        private const string WrongAssetId = "ETH";

        private IExternalBalanceServicesSettings GetExternalBalanceServicesSettings()
        {
            var mock = new Mock<IExternalBalanceServicesSettings>();
            mock.Setup(s => s.Balances).Returns(new[]
            {
                new ExternalBalanceServiceSettings()
                {
                    Source = Source,
                    AssetValues = new[]
                    {
                        new AssetValueSettings()
                        {
                            Asset = AssetId,
                            Value = DefaultBalance
                        }
                    }
                }
            });
            return mock.Object;
        }

        [Fact]
        public void GetAssetBalance_WhenRepositoryContainsBalance_ReturnsBalance()
        {
            var task = new Task<IAssetBalance>(() => new AssetBalance(Source, AssetId)
            {
                Value = Balance
            });
            var assetBalanceRepositoryMock = new Mock<IAssetBalanceRepository>();
            assetBalanceRepositoryMock.Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(task);
            var externalBalanceService = new ExternalBalanceService(assetBalanceRepositoryMock.Object,
                GetExternalBalanceServicesSettings());

            externalBalanceService.Start();
            decimal result = externalBalanceService.GetAssetBalance(Source, AssetId);

            Assert.Equal(result, Balance);
        }

        [Fact]
        public void GetAssetBalance_WhenRepositoryDoesNotContainBalance_ReturnsDefaultBalance()
        {
            var assetBalanceRepositoryMock = new Mock<IAssetBalanceRepository>();
            var externalBalanceService = new ExternalBalanceService(assetBalanceRepositoryMock.Object,
                GetExternalBalanceServicesSettings());

            externalBalanceService.Start();
            decimal result = externalBalanceService.GetAssetBalance(Source, AssetId);

            Assert.Equal(result, DefaultBalance);
        }

        [Fact]
        public async Task AddAssetAsync_WhenAddValue_UpdateRepository()
        {
            var assetBalanceRepositoryMock = new Mock<IAssetBalanceRepository>();
            var externalBalanceService = new ExternalBalanceService(assetBalanceRepositoryMock.Object,
                GetExternalBalanceServicesSettings());

            externalBalanceService.Start();
            await externalBalanceService.AddAssetAsync(Source, AssetId, Balance);

            assetBalanceRepositoryMock.Verify(r=>r.InsertOrReplaceAsync(It.Is<IAssetBalance>(b=> 
                    b.Source == Source 
                    && b.AssetId == AssetId 
                    && b.Value == Balance + DefaultBalance)), 
                Times.Once);
        }

        [Fact]
        public async Task AddAssetAsync_WhenAddValue_SetNewValue()
        {
            var assetBalanceRepositoryMock = new Mock<IAssetBalanceRepository>();
            var externalBalanceService = new ExternalBalanceService(assetBalanceRepositoryMock.Object,
                GetExternalBalanceServicesSettings());

            externalBalanceService.Start();
            await externalBalanceService.AddAssetAsync(Source, AssetId, Balance);
            decimal result = externalBalanceService.GetAssetBalance(Source, AssetId);

            Assert.Equal(result, DefaultBalance + Balance);
        }

        [Fact]
        public async Task AddAssetAsync_WhenWrongAssetId_ExceptionIsThrown()
        {
            var assetBalanceRepositoryMock = new Mock<IAssetBalanceRepository>();
            var externalBalanceService = new ExternalBalanceService(assetBalanceRepositoryMock.Object,
                GetExternalBalanceServicesSettings());

            externalBalanceService.Start();

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
                await externalBalanceService.AddAssetAsync(Source, WrongAssetId, Balance));
        }
    }
}
