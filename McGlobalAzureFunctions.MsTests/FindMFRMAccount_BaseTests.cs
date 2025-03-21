using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;

using McGlobalAzureFunctions.Abstractions.Fox;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace McGlobalAzureFunctions
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FindMFRMAccount_BaseTests
    {
        protected Mock<ILogger<FindMFRMAccount>> mockLogger;
        protected Mock<IOrganizationServiceAsync2> mockSvcClient;
        protected Mock<IFoxProvider> mockFoxProvider;
        public FindMFRMAccount? findMfrmAccObject;

        [TestInitialize]
        public void TestInitialize()
        {
            mockLogger = new Mock<ILogger<FindMFRMAccount>>();
            mockFoxProvider = new Mock<IFoxProvider>();
            mockSvcClient = new Mock<IOrganizationServiceAsync2>();

            this.findMfrmAccObject = new FindMFRMAccount(this.mockLogger.Object, this.mockSvcClient.Object, mockFoxProvider.Object);
        }
    }
}
