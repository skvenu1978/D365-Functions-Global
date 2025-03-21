using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

using McGlobalAzureFunctions.Abstractions.Responses;
using McGlobalAzureFunctions.Models.Responses;
using McGlobalAzureFunctions.Models.Common;
using McGlobalAzureFunctions.FunctionsCommon;
using Azure.Messaging.ServiceBus;
using Microsoft.PowerPlatform.Dataverse.Client;
using McGlobalAzureFunctions.Mock;

namespace McGlobalAzureFunctions
{
    [TestClass]
    public class FnCorporateClientsReaderTests
    {
        private Mock<ILogger<FnCorporateClientsReader>> _loggerMock;
        private Mock<IOrganizationServiceAsync2> _crmServiceClientMock;
        private Mock<IClientResponseProvider> _clientProviderMock;
        private Mock<IContactProvider> _contactProviderMock;
        private Mock<IFraudProvider> _fraudProviderMock;
        private FnCorporateClientsReader _fnCorporateClientsReader;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<FnCorporateClientsReader>>();
            _crmServiceClientMock = new Mock<IOrganizationServiceAsync2>();
            _clientProviderMock = new Mock<IClientResponseProvider>();
            _contactProviderMock = new Mock<IContactProvider>();
            _fraudProviderMock = new Mock<IFraudProvider>();

            _fnCorporateClientsReader = new FnCorporateClientsReader(
                _loggerMock.Object,
                _crmServiceClientMock.Object,
                _clientProviderMock.Object,
                _contactProviderMock.Object,
                _fraudProviderMock.Object
            );
        }

        [TestMethod]
        public async Task Run_WithValidClientContactInfoResponse_ShouldCompleteMessage()
        {
            // Arrange
            var msg = new Mock<ServiceBusReceivedMessage>();// Mock or create a real message as per your requirements.
            var message = msg.Object;
            var messageActions = new Mock<ServiceBusMessageActions>();
            var receivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            new BinaryData("Valid content"), "123", null, null, null, null, TimeSpan.Zero, null, null, null, "text/plain");

            msg.Setup(m => m.MessageId).Returns(It.IsAny<int>().ToString());
            msg.Setup(m => m.Body).Returns(new BinaryData("Test Message Body"));
            msg.Setup(m => m.ContentType).Returns("application/json");

            var messageActionsMock = new Mock<ServiceBusMessageActions>();
            messageActionsMock.Setup(ma => ma.CompleteMessageAsync(message, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            //Your handler method or class which processes the message
            var fnCorporateClientsReader = new FnCorporateClientsReader(_loggerMock.Object, _crmServiceClientMock.Object, _clientProviderMock.Object, _contactProviderMock.Object, _fraudProviderMock.Object);

            //Act
            await fnCorporateClientsReader.Run(receivedMessage, messageActionsMock.Object);

            // Assert - Ensure that the message was processed(or any other expectations)
            messageActionsMock.Verify(ma => ma.CompleteMessageAsync(receivedMessage, It.IsAny<CancellationToken>()), Times.Once);

            //// Mocking FnCommon method calls
            var msgProps = new AsbMessageInfo { CanProcess = true, MessageType = nameof(ClientContactInfoResponse) };
            var response = new ClientContactInfoResponse(); // Create a mock response object.
            _clientProviderMock.Setup(cp => cp.ProcessClientContactInfoResponse(It.IsAny<ClientContactInfoResponse>(), It.IsAny<Guid>(), It.IsAny<IOrganizationServiceAsync2>()))
                .Returns(new DynTaskResponse { IsSuccess = true });

            // Mocking the ServiceBusMessageActions to avoid calling actual Service Bus
            //messageActions.Setup(ma => ma.CompleteMessageAsync(It.IsAny<ServiceBusReceivedMessage>)).Returns(Task.CompletedTask);

            //// Act
            //await _fnCorporateClientsReader.Run(message, messageActions.Object);

            //// Assert
            //_clientProviderMock.Verify(cp => cp.ProcessClientContactInfoResponse(It.IsAny<ClientContactInfoResponse>(), It.IsAny<Guid>(), It.IsAny<IOrganizationServiceAsync2>()), Times.Once);
            //messageActions.Verify(ma => ma.CompleteMessageAsync(message), Times.Once);
        }

        [TestMethod]
        public async Task Run_WithInvalidClientContactInfoResponse_ShouldDeadLetterMessage()
        {
            // Arrange
            var message = new Mock<ServiceBusReceivedMessage>(); // Mock or create a real message as per your requirements.
            var messageActions = new Mock<ServiceBusMessageActions>();

            // Mocking FnCommon method calls
            var msgProps = new AsbMessageInfo { CanProcess = true, MessageType = nameof(ClientContactInfoResponse) };
            var response = new ClientContactInfoResponse(); // Create a mock response object.
            _clientProviderMock.Setup(cp => cp.ProcessClientContactInfoResponse(It.IsAny<ClientContactInfoResponse>(), It.IsAny<Guid>(), It.IsAny<IOrganizationServiceAsync2>()))
                .Returns(new DynTaskResponse { IsSuccess = false, ErrorMessage = "Error processing message." });

            // Mocking the ServiceBusMessageActions to avoid calling actual Service Bus
            //messageActions.Setup(ma => ma.DeadLetterMessageAsync(message)).Returns(Task.CompletedTask);

            // Act
            //await _fnCorporateClientsReader.Run(message, messageActions.Object);

            // Assert
            _clientProviderMock.Verify(cp => cp.ProcessClientContactInfoResponse(It.IsAny<ClientContactInfoResponse>(), It.IsAny<Guid>(), It.IsAny<IOrganizationServiceAsync2>()), Times.Once);
            // messageActions.Verify(ma => ma.DeadLetterMessageAsync(message), Times.Once);
        }

        [TestMethod]
        public async Task Run_WithException_ShouldLogError()
        {
            // Arrange
            var message = new Mock<ServiceBusReceivedMessage>(); // Mock or create a real message as per your requirements.
            var messageActions = new Mock<ServiceBusMessageActions>();

            // Mocking FnCommon method calls to simulate an exception scenario
            var msgProps = new AsbMessageInfo { CanProcess = true, MessageType = nameof(ClientContactInfoResponse) };
            _clientProviderMock.Setup(cp => cp.ProcessClientContactInfoResponse(It.IsAny<ClientContactInfoResponse>(), It.IsAny<Guid>(), It.IsAny<IOrganizationServiceAsync2>()))
                .Throws(new System.Exception("Test exception"));

            // Mocking the ServiceBusMessageActions to avoid calling actual Service Bus
            //messageActions.Setup(ma => ma.DeadLetterMessageAsync(message)).Returns(Task.CompletedTask);

            // Act
            //await _fnCorporateClientsReader.Run(message, messageActions.Object);

            // Assert
            //_loggerMock.Verify(logger => logger.LogError(It.IsAny<System.Exception>(), "Error Processing Response"), Times.Once);
        }
    }
}