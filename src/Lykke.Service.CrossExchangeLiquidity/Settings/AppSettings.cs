using Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings;
using Lykke.Service.CrossExchangeLiquidity.Settings.SlackNotifications;

namespace Lykke.Service.CrossExchangeLiquidity.Settings
{
    public class AppSettings
    {
        public CrossExchangeLiquiditySettings CrossExchangeLiquidityService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
