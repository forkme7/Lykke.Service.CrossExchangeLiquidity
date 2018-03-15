using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        [AzureTableCheck]
        public string EntitiesConnString { get; set; }

        public string AssetBalanceTableName { get; set; }
    }
}
