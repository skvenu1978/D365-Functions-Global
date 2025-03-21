using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;

namespace McGlobalAzureFunctions.Mock
{
    public class ServiceBusMessageActionsWrapper : IServiceBusMessageActions
    {
        private readonly ServiceBusMessageActions _messageActions;

        public ServiceBusMessageActionsWrapper(ServiceBusMessageActions messageActions)
        {
            _messageActions = messageActions;
        }

        public Task CompleteMessageAsync(ServiceBusReceivedMessage message)
        {
            return _messageActions.CompleteMessageAsync(message);
        }

        public Task DeadLetterMessageAsync(ServiceBusReceivedMessage message)
        {
            return _messageActions.DeadLetterMessageAsync(message);
        }
    }
}
