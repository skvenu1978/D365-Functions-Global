// <copyright file="ContactWebActivationConversion.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Converters
{
    using DAL;
    using Models.MEX;
    using Models.Requests;
    using Const;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using System.ServiceModel;

    public class ContactWebActivationConversion
    {
        /// <summary>
        /// GetContactWebActivationtRequest.
        /// </summary>
        /// <param name="contextEntity"></param>
        /// <param name="svcClient"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static ContactWebActivationInfoRequest GetContactWebActivationtRequest(Entity contextEntity, IOrganizationServiceAsync2 svcClient, out string errMsg)
        {
            ContactWebActivationInfoRequest contactWebActivationInfoRequest = new ContactWebActivationInfoRequest();
            errMsg = string.Empty;

            try
            {
                int? omniContactId = AttributeHelper.GetStringAttributeAsIntegerValue(contextEntity, ContactConstants.Mcorp_omnicontactid);
                Guid? parentCustomerId = AttributeHelper.GetGuidFromEntityReference(contextEntity, ContactConstants.Parentcustomerid);

                if (parentCustomerId.HasValue)
                {
                    Entity parentAccount = svcClient.Retrieve(AccountConstants.Account, parentCustomerId.Value, new Microsoft.Xrm.Sdk.Query.ColumnSet(AccountConstants.Mcorp_fedsid));
                    int? fedsId = AttributeHelper.GetStringAttributeAsIntegerValue(parentAccount, AccountConstants.Mcorp_fedsid);

                    if (omniContactId.HasValue && fedsId.HasValue)
                    {
                        ContactWebActivationInfo contactWebActivationInfo = new ContactWebActivationInfo();
                        contactWebActivationInfo.CRMContactGuid = contextEntity.Id;
                        contactWebActivationInfo.ClientId = fedsId.Value;
                        contactWebActivationInfo.ContactId = omniContactId.Value;

                        contactWebActivationInfoRequest.ContactWebActivations.Add(contactWebActivationInfo);
                    }
                }
                else
                {
                    errMsg = "Contact does not have Parent account";
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                errMsg = "Failed to GetRequest for ContactWebActivation:" + contextEntity.Id.ToString() + " with error:" + EntityCommon.GetErrorMessageFromFaultException(ex);
            }

            return contactWebActivationInfoRequest;
        }
    }
}