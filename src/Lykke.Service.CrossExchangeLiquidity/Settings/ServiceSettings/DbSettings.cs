using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.CrossExchangeLiquidity.Settings.ServiceSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
