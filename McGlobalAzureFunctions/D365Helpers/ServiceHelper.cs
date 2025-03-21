// <copyright file="ServiceHelper.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Microsoft.PowerPlatform.Dataverse.Client;

    public class ServiceHelper
    {
        public static ServiceClient? GetServiceClient(string d365ConnString, out string errMsg)
        {
            ServiceClient? svcClient = null;
            errMsg = string.Empty;

            try
            {
                svcClient = new ServiceClient(d365ConnString);

                if (!svcClient.IsReady)
                {
                    errMsg = "Failed to authenticate D365:" + svcClient.LastException.Message;
                }
            }
            catch (Exception ex)
            {
                errMsg = "Failed to authenticate D365:" + ex.Message;

                if (ex.InnerException != null)
                {
                    errMsg = "Failed to authenticate D365:" + ex.InnerException.Message;
                }
            }

            return svcClient;
        }
    }
}