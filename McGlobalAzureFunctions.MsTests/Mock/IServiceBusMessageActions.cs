using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Mock
{
    public interface IServiceBusMessageActions
    {
        Task CompleteMessageAsync(ServiceBusReceivedMessage message);
        Task DeadLetterMessageAsync(ServiceBusReceivedMessage message);
    }
}
