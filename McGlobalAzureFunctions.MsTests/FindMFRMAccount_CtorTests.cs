using System.Diagnostics.CodeAnalysis;

namespace McGlobalAzureFunctions
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FindMFRMAccount_CtorTests : FindMFRMAccount_BaseTests
    {
        [TestMethod]
        public void Ctor_NullCrmServiceClient_ShouldThrowArgumentNullException()
        {
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() => new FindMFRMAccount(this.mockLogger.Object, null, this.mockFoxProvider.Object));
            Assert.AreEqual("crmServiceClient", ex.ParamName, ignoreCase: false);
        }

        [TestMethod]
        public void Ctor_NullFoxProvider_ShouldThrowArgumentNullException()
        {
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() => new FindMFRMAccount(this.mockLogger.Object, this.mockSvcClient.Object, null));
            Assert.AreEqual("foxProvider", ex.ParamName, ignoreCase: false);
        }

        [TestMethod]
        public void Ctor_NullLogger_ShouldThrowArgumentNullException()
        {
            ArgumentNullException ex = Assert.ThrowsException<ArgumentNullException>(() => new FindMFRMAccount(null, this.mockSvcClient.Object, this.mockFoxProvider.Object));
            Assert.AreEqual("logger", ex.ParamName, ignoreCase: false);
        }
    }
}
