using AutoMapper;
using AutoMapper.Configuration;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Core.Domain.LykkeOrderBook;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.OrderBook;
using Lykke.Service.CrossExchangeLiquidity.Shared.Models.Orders;

namespace Lykke.Service.CrossExchangeLiquidity.Modules
{
    public class MapperProvider
    {
        public IMapper GetMapper()
        {
            var mce = new MapperConfigurationExpression();

            mce.CreateMap<VolumePrice, VolumePriceModel>();
            mce.CreateMap<OrderBook, OrderBookModel>();
            mce.CreateMap<SourcedVolumePrice, SourcedVolumePriceModel>();
            mce.CreateMap<ICompositeOrderBook, CompositeOrderBookModel>();

            mce.CreateMap<MultiLimitOrderModel, LykkeMultiLimitOrderModel>();
            mce.CreateMap<MultiOrderItemModel, LykkeMultiOrderItemModel>();
            mce.CreateMap<LimitOrderFeeModel, LykkeLimitOrderFeeModel>();

            var mc = new MapperConfiguration(mce);
            mc.AssertConfigurationIsValid();

            return new Mapper(mc);
        }
    }
}
