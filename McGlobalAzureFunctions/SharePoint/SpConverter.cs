// <copyright file="SpConverter.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.SharePoint
{
    using McGlobalAzureFunctions.Const;

    public class SpConverter
    {
        /// <summary>
        /// Reads the CONFIG
        /// and returns the SharePoint Id
        /// </summary>
        /// <returns></returns>
        public static string GetSpSiteIdBasedOnSiteName(string spSiteName)
        {
            string spSiteId = string.Empty;

            if (!string.IsNullOrEmpty(spSiteName))
            {
                switch (spSiteName)
                {
                    case "MicrosoftDynamics365-Development":
                        spSiteId = SharePointConstants.SpSiteId_Dev;
                        break;
                    case "MicrosoftDynamics365-CorpDev":
                        spSiteId = SharePointConstants.SpSiteId_CorpDev;
                        break;
                    case "MicrosoftDynamics365-QA":
                        spSiteId = SharePointConstants.SpSiteId_QA;
                        break;
                    case "MicrosoftDynamics365-QA-Auto":
                        spSiteId = SharePointConstants.SpSiteId_QA_Auto;
                        break;
                    case "MicrosoftDynamics365-UAT":
                        spSiteId = SharePointConstants.SpSiteId_UAT;
                        break;
                    case "MicrosoftDynamics365-PreProd":
                        spSiteId = SharePointConstants.SpSiteId_PreProd;
                        break;
                    case "MicrosoftDynamics365-Prod":
                        spSiteId = SharePointConstants.SpSiteId_Prod;
                        break;
                }
            }

            return spSiteId;
        }
    }
}