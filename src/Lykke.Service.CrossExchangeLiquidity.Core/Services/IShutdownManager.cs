using System.Threading.Tasks;
using Common;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();

        void Register(IStopable stopable);
    }
}
