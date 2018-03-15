﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.CrossExchangeLiquidity.Core.Domain.ExternalExchange
{
    public interface IAssetBalance
    {
        string Source { get; }
        string AssetId { get; }
        decimal Value { get; set; }
    }
}
