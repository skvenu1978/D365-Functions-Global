// <copyright file="AttachmentProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Responses
{
    using McGlobalAzureFunctions.Abstractions.Responses;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Models.Common;
    using McGlobalAzureFunctions.Models.MEX;
    using McGlobalAzureFunctions.SharePoint;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    /// <summary>
    /// Main class to handle
    /// attachment requests
    /// </summary>
    public class AttachmentProvider : IAttachmentProvider
    {
        /// <summary>
        /// Main method to upload attachments to SharePoint.
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="creds">Sp Credentials</param>
        /// <param name="spSiteName">Sp Site Name</param>
        /// <param name="requestId">requestId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>DynTaskResponse</returns>
        public DynTaskResponse ProcessAttachmentRequest(AttachmentRequest request, SpCredentials creds, string spSiteName, Guid requestId, IOrganizationServiceAsync2 svcClient)
        {
            DynTaskResponse response = new ();
            response.IsSuccess = false;
            response.RecordId = null;
            string spFolderName = string.Empty;
            Entity entity = new ();

            if (request.EntityName == null)
            {
                response.ErrorMessage = "request parameter is null";
                return response;
            }

            try
            {
                entity = svcClient.Retrieve(request.EntityName, request.EntityId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                response.ErrorMessage = "ProcessAttachmentRequest failed to get related entity:" + EntityCommon.GetErrorMessageFromException(ex);
                response.Exception = ex;
            }

            if (!string.IsNullOrEmpty(entity.LogicalName))
            {
                string attributeName = "fullname";

                if (entity.LogicalName == "account")
                {
                    attributeName = "name";
                }

                string recordName = entity.GetAttributeValue<string>(attributeName);
                string cleanGuid = entity.Id.ToString().Replace("-", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
                spFolderName = recordName + "_" + cleanGuid.ToUpper();

                if (!string.IsNullOrEmpty(spSiteName))
                {
                    string spRecordFolder = GetDocumentLocationFolder(spFolderName, entity.LogicalName, entity.Id, svcClient);

                    // check create Omni document location in D365
                    if (request.EntityName.ToLower() == AccountConstants.Account)
                    {
                        response = SpFileHandler.UploadAttachmentsToSharePointOMNIFolder(spSiteName, creds, SharePointConstants.SharePointAccountFolder, spRecordFolder, request);
                    }

                    if (request.EntityName.ToLower() == ContactConstants.Contact)
                    {
                        response = SpFileHandler.UploadAttachmentsToSharePointOMNIFolder(spSiteName, creds, SharePointConstants.SharePointContactFolder, spRecordFolder, request);
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Creates new document location if it does not exist.
        /// </summary>
        /// <param name="newRecordFolder">newRecordFolder</param>
        /// <param name="entityName">entityName</param>
        /// <param name="entityId">entityId</param>
        /// <param name="svcClient">svcClient</param>
        /// <returns>string</returns>
        private static string GetDocumentLocationFolder(string newRecordFolder, string entityName, Guid entityId, IOrganizationServiceAsync2 svcClient)
        {
            string spRecordFolder = string.Empty;
            Guid? entityDocLocationId = GetEntityDocumentLocationId(entityName, svcClient);

            if (entityDocLocationId.HasValue)
            {
                Guid? recordDocLocationId = null;
                string fetchXml = FetchStrings.GetLinkedMainDocumentLocation(entityName, entityId.ToString(), entityDocLocationId.Value.ToString());
                EntityCollection coll = FetchXmlHelper.GetDataUsingFetch(svcClient, fetchXml);

                if (coll != null && coll.Entities.Count > 0)
                {
                    spRecordFolder = AttributeHelper.GetStringAttributeValue(coll.Entities[0], "relativeurl");
                    recordDocLocationId = coll.Entities[0].Id;
                }

                if (string.IsNullOrEmpty(spRecordFolder))
                {
                    // create record linked doc location
                    Entity docLocation = new Entity(SharePointConstants.SpDocLocationEntity);
                    docLocation["name"] = newRecordFolder;
                    docLocation["parentsiteorlocation"] = new EntityReference(SharePointConstants.SpDocLocationEntity, entityDocLocationId.Value);
                    docLocation["relativeurl"] = newRecordFolder;
                    docLocation["locationtype"] = new OptionSetValue(0); // GENERAL
                    docLocation["regardingobjectid"] = new EntityReference(entityName, entityId);
                    recordDocLocationId = svcClient.Create(docLocation);
                    spRecordFolder = newRecordFolder;

                    // create OMNI Document folder
                    Entity omniDocLocation = new Entity(SharePointConstants.SpDocLocationEntity);
                    omniDocLocation["name"] = SharePointConstants.SpOMNIFolder;
                    omniDocLocation["parentsiteorlocation"] = new EntityReference(SharePointConstants.SpDocLocationEntity, recordDocLocationId.Value);
                    omniDocLocation["relativeurl"] = SharePointConstants.SpOMNIFolder;
                    omniDocLocation["description"] = "Sub-folder of " + newRecordFolder;
                    omniDocLocation["locationtype"] = new OptionSetValue(0); // GENERAL
                    omniDocLocation["regardingobjectid"] = new EntityReference(entityName, entityId);
                    Guid omniDocLocationId = svcClient.Create(omniDocLocation);
                }
                else
                {
                    // check doc location with Omni Documents exists
                    string omniDocLocationfetchXml = FetchStrings.GetOmniDocumentsLocation(entityName, entityId.ToString(), entityDocLocationId.Value.ToString());
                    EntityCollection omniDocColl = FetchXmlHelper.GetDataUsingFetch(svcClient, omniDocLocationfetchXml);

                    if (omniDocColl.Entities.Count == 0 && recordDocLocationId.HasValue)
                    {
                        Entity omniDocLocation = new Entity(SharePointConstants.SpDocLocationEntity);
                        omniDocLocation["name"] = SharePointConstants.SpOMNIFolder;
                        omniDocLocation["parentsiteorlocation"] = new EntityReference(SharePointConstants.SpDocLocationEntity, recordDocLocationId.Value);
                        omniDocLocation["relativeurl"] = SharePointConstants.SpOMNIFolder;
                        omniDocLocation["description"] = "Sub-folder of " + spRecordFolder;
                        omniDocLocation["locationtype"] = new OptionSetValue(0);// GENERAL
                        omniDocLocation["regardingobjectid"] = new EntityReference(entityName, entityId);
                        Guid omniDocLocationId = svcClient.Create(omniDocLocation);
                    }
                }
            }

            return spRecordFolder;
        }

        /// <summary>
        /// Get Entity level
        /// Document Location Id
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="svcClient"></param>
        /// <returns></returns>
        private static Guid? GetEntityDocumentLocationId(string entityName, IOrganizationServiceAsync2 svcClient)
        {
            Guid? parentDocLocationId = null;

            string parentFetchXml = FetchStrings.GetParentDocumentLocation(entityName);
            EntityCollection parentColl = FetchXmlHelper.GetDataUsingFetch(svcClient, parentFetchXml);

            if (parentColl != null && parentColl.Entities.Count > 0)
            {
                parentDocLocationId = parentColl.Entities[0].Id;
            }

            if (!parentDocLocationId.HasValue)
            {
                // get default root sharepoint site id
                string rootSiteFetchXml = FetchStrings.GetRootSharePointSite();

                EntityCollection rootSiteColl = FetchXmlHelper.GetDataUsingFetch(svcClient, rootSiteFetchXml);
                Guid? rootSiteId = null;

                if (rootSiteColl != null && rootSiteColl.Entities.Count > 0)
                {
                    rootSiteId = rootSiteColl.Entities[0].Id;

                    // Create document location for the ENTITY
                    Entity docLocation = new Entity(SharePointConstants.SpDocLocationEntity);
                    docLocation["name"] = entityName + " Site";
                    docLocation["parentsiteorlocation"] = new EntityReference(SharePointConstants.SpSiteEntity, rootSiteId.Value);
                    docLocation["relativeurl"] = entityName.ToLower();
                    docLocation["locationtype"] = new OptionSetValue(0);// GENERAL
                    parentDocLocationId = svcClient.Create(docLocation);
                }
            }

            return parentDocLocationId;
        }

        private (string errMsg, string mainPhone, string phone2, string cname) SetExistingLinkedPrimaryAddressToHistoric(string contactId, IOrganizationServiceAsync2 svcClient)
        {
            string errMsg = string.Empty;
            string mainPhone = string.Empty;
            string phone2 = string.Empty;
            string cname = string.Empty;

            string fetchXml = FetchStrings.GetContactRelatedPrimaryAddress(contactId);

            try
            {
                EntityCollection coll = FetchXmlHelper.GetDataUsingFetch(svcClient, fetchXml);

                if (coll != null && coll.Entities.Count > 0)
                {
                    Guid? addressToUpdateId = null;
                    addressToUpdateId = coll.Entities[0].Id;
                    mainPhone = AttributeHelper.GetStringAttributeValue(coll.Entities[0], MoneycorpAddressConstants.mcorp_mainphone);
                    phone2 = AttributeHelper.GetStringAttributeValue(coll.Entities[0], MoneycorpAddressConstants.mcorp_phone2);
                    cname = AttributeHelper.GetStringAttributeValue(coll.Entities[0], MoneycorpAddressConstants.mcorp_name);

                    Entity address = new Entity(MoneycorpAddressConstants.mcorp_moneycorpaddress, addressToUpdateId.Value);
                    //address[MoneycorpAddressConstants.mcorp_addresstype] = new OptionSetValue((int)MoneycorpAddressType.Historic);
                    svcClient.Update(address);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "SetExistingLinkedPrimaryAddressToHistoric-Failed to update entity:" + EntityCommon.GetErrorMessageFromException(ex);
            }

            return (errMsg, mainPhone, phone2, cname);
        }
    }
}