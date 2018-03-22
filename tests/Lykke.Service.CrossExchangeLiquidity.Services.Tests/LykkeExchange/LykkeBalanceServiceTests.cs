using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.CrossExchangeLiquidity.Core.Settings.LykkeExchange;
using Lykke.Service.CrossExchangeLiquidity.Services.LykkeExchange;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lykke.Service.CrossExchangeLiquidity.Services.Tests.LykkeExchange
{
    public class LykkeBalanceServiceTests
    {
        private const string AssetId = "BTC";
        private const double DefaultBalance = 2;
        private const decimal Balance = 1;
        private const double Reserved = 0;
        private static readonly TimeSpan TimeSpan = new TimeSpan(0, 0, 1);

        private ILykkeBalanceServiceSettings GetSettings()
        {
            var settings = new Mock<ILykkeBalanceServiceSettings>();
            settings.Setup(s => s.AssetIds).Returns(new[] {AssetId});
            settings.Setup(s => s.TimeSpan).Returns(TimeSpan);
            return settings.Object;
        }

        [Fact]
        public async Task AddAssetAsync_WhenAddValue_SetNewValue()
        {
            var balancesClient = new Mock<IBalancesClient>();
            balancesClient.Setup(c => c.GetClientBalances(It.IsAny<string>()))
                .Returns(() => Task.FromResult(new[]
                {
                    new ClientBalanceResponseModel(DefaultBalance, Reserved, AssetId)
                }.AsEnumerable()));

            var lykkeBalanceService = new LykkeBalanceService(balancesClient.Object, GetSettings());

            lykkeBalanceService.Start();
            await lykkeBalanceService.AddAssetAsync(AssetId, Balance);
            decimal result = lykkeBalanceService.GetAssetBalance(AssetId);

            Assert.Equal(result, Balance + (decimal) DefaultBalance);
        }

        [Fact]
        public void GetAssetBalance_WhenNoValue_ReturnsZero()
        {
            var balancesClient = new Mock<IBalancesClient>();

            var lykkeBalanceService = new LykkeBalanceService(balancesClient.Object, GetSettings());

            lykkeBalanceService.Start();
            decimal result = lykkeBalanceService.GetAssetBalance(AssetId);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetFromServerAsync_WhenBalanceIsReceivedFromServer_BalanceIsOverwritten()
        {
            var balancesClient = new Mock<IBalancesClient>();
            balancesClient.Setup(c => c.GetClientBalances(It.IsAny<string>()))
                .Returns(() => Task.FromResult(new[]
                {
                    new ClientBalanceResponseModel(DefaultBalance, Reserved, AssetId)
                }.AsEnumerable()));

            var lykkeBalanceService = new LykkeBalanceService(balancesClient.Object, GetSettings());

            lykkeBalanceService.Start();
            await lykkeBalanceService.AddAssetAsync(AssetId, Balance);
            await Task.Delay(new TimeSpan(TimeSpan.Ticks + 100));
            decimal result = lykkeBalanceService.GetAssetBalance(AssetId);

            Assert.Equal((decimal) DefaultBalance, result);
        }
    }
}
