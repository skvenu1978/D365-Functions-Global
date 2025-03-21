// <copyright file="FetchXmlHelper.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions
{
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Common function to
    /// retrieve data from D365 using
    /// FetchXml.
    /// </summary>
    public class FetchXmlHelper
    {
        /// <summary>
        /// GetDataUsingFetch.
        /// </summary>
        /// <param name="service">service</param>
        /// <param name="fetchXml">fetchXml</param>
        /// <returns>EntityCollection</returns>
        public static EntityCollection GetDataUsingFetch(IOrganizationServiceAsync2 service, string fetchXml)
        {
            EntityCollection returnBulkCollection = new EntityCollection();

            int fetchCount = 5000;
            int pageNumber = 1;
            int recordCount = 0;
            string? pagingCookie = null;

            while (true)
            {
                string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                RetrieveMultipleRequest fetchRequest1 = new RetrieveMultipleRequest
                {
                    Query = new FetchExpression(xml)
                };

                var response = (RetrieveMultipleResponse)service.Execute(fetchRequest1);
                EntityCollection returnCollection = response.EntityCollection;
                int x = returnCollection.Entities.Count;

                foreach (var c in returnCollection.Entities)
                {
                    returnBulkCollection.Entities.Add(c);
                    recordCount++;
                }

                // Check for morerecords, if it returns 1.
                if (returnCollection.MoreRecords)
                {
                    pageNumber++;
                    pagingCookie = returnCollection.PagingCookie;
                }
                else
                {
                    // If no more records in the result nodes, exit the loop.
                    break;
                }
            }

            return returnBulkCollection;
        }

        /// <summary>
        /// Create XML.
        /// </summary>
        /// <param name="xml">xml</param>
        /// <param name="cookie">cookie</param>
        /// <param name="page">page</param>
        /// <param name="count">count</param>
        /// <returns>string</returns>
        private static string CreateXml(string xml, string? cookie, int page, int count)
        {
            StringReader stringReader = new (xml);
            XmlTextReader reader = new (stringReader);
            XmlDocument doc = new ();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        /// <summary>
        /// Create XML with doc.
        /// </summary>
        /// <param name="doc">doc</param>
        /// <param name="cookie">cookie</param>
        /// <param name="page">page</param>
        /// <param name="count">count</param>
        /// <returns>string</returns>
        private static string CreateXml(XmlDocument doc, string? cookie, int page, int count)
        {
            string retValue = string.Empty;

            if (doc.DocumentElement != null)
            {
                XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

                if (cookie != null)
                {
                    XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                    pagingAttr.Value = cookie;
                    attrs.Append(pagingAttr);
                }

                XmlAttribute pageAttr = doc.CreateAttribute("page");
                pageAttr.Value = page.ToString();
                attrs.Append(pageAttr);
                XmlAttribute countAttr = doc.CreateAttribute("count");
                countAttr.Value = count.ToString();
                attrs.Append(countAttr);
                StringBuilder sb = new (1024);
                StringWriter stringWriter = new (sb);
                XmlTextWriter writer = new (stringWriter);
                doc.WriteTo(writer);
                writer.Close();
                retValue = sb.ToString();
            }

            return retValue;
        }
    }
}