

namespace Metatrader4ClientApp.Infrastructure
{
    using Metatrader4ClientApp.Infrastructure.Models;
    using Prism.Events;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using TradingAPI.MT4Server;

   
    public class ConnectionParameterCreatedEvent : PubSubEvent<ConnectionParameter>
    {
    }

    public class TradeListUpdatedEvent : PubSubEvent<IDictionary<ConnectionParameter, Order[]>>
    {
    }

    

}
