// <copyright file="LexisNexisUSProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Ekyc
{
    using McGlobalAzureFunctions.Abstractions.Ekyc;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Models.Ekyc;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation for ILexisNexisUSProvider
    /// </summary>
    public class LexisNexisUSProvider : ILexisNexisUSProvider
    {
        /// <summary>
        /// CreateRequest.
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="serviceClient"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public IDnVRequest? CreateRequest(string contactId, IOrganizationServiceAsync2 serviceClient, out string errorMessage)
        {
            try
            {
                Entity contact = serviceClient.Retrieve("contact", Guid.Parse(contactId), new ColumnSet(true));
                var idnvRequest = new IDnVRequest();
                idnvRequest.FirstName = contact[LexNexConstants.att_firstName].ToString();
                idnvRequest.LastName = contact[LexNexConstants.att_lastname].ToString();
                idnvRequest.ContactGuid = contact.Id.ToString();
                idnvRequest.PhoneNumber = contact.Contains(LexNexConstants.att_phone) ? contact[LexNexConstants.att_phone].ToString() : "";
                idnvRequest.NationalId = contact.Contains(LexNexConstants.att_nationalId) ? contact[LexNexConstants.att_nationalId].ToString() : "";

                QueryExpression query = new QueryExpression("mcorp_moneycorpaddress")
                {
                    ColumnSet = new ColumnSet(true)
                };

                query.Criteria.AddCondition(new ConditionExpression("mcorp_parentid", ConditionOperator.Equal, contact.Id));
                query.Criteria.AddCondition(new ConditionExpression("mcorp_addresstype", ConditionOperator.Equal, 1));
                EntityCollection results = serviceClient.RetrieveMultiple(query);

                foreach (Entity entity in results.Entities)
                {
                    Entity country = serviceClient.Retrieve("mcorp_country", entity.GetAttributeValue<EntityReference>(LexNexConstants.att_country).Id, new ColumnSet(true));
                    idnvRequest.AddressLine1 = entity.Contains(LexNexConstants.att_buildingNumber) ? entity[LexNexConstants.att_buildingNumber].ToString() : "";
                    idnvRequest.AddressLine2 = entity.Contains(LexNexConstants.att_street) ? entity[LexNexConstants.att_street].ToString() : "";
                    idnvRequest.City = entity.Contains(LexNexConstants.att_city) ? entity[LexNexConstants.att_city].ToString() : "";
                    idnvRequest.ZipOrPostCode = entity.Contains(LexNexConstants.att_postal) ? entity[LexNexConstants.att_postal].ToString() : "";
                    idnvRequest.StateOrCounty = entity.Contains(LexNexConstants.att_county) ? entity[LexNexConstants.att_county].ToString() : "";
                }

                idnvRequest.PhoneNumber = contact.Contains(LexNexConstants.att_phone) ? contact[LexNexConstants.att_phone].ToString() : "";

                if (contact.Contains(LexNexConstants.att_dob))
                {
                    idnvRequest.DateOfBirth = contact.GetAttributeValue<DateTime>(LexNexConstants.att_dob);
                }

                errorMessage = string.Empty;
                return idnvRequest;
            }
            catch (Exception)
            {
                errorMessage = "Error in Request";
                return null;
            }
        }

        /// <summary>
        /// DoeKYC.
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="restURL"></param>
        /// <param name="contactId"></param>
        /// <param name="serviceClient"></param>
        /// <returns></returns>
        public async Task<ScreeningApiResult> DoeKYC(string base64String, string restURL, string contactId, IOrganizationServiceAsync2 serviceClient)
        {
            string errorMessage = string.Empty;
            var apiResponse = new ScreeningApiResult();
            var requestPayload = CreateRequest(contactId, serviceClient, out errorMessage);

            try
            {
                if (requestPayload != null)
                {
                    var requestString = LexisNexisUSRequest.ToJson(requestPayload);
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, restURL);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
                    var content = new StringContent(requestString, null, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var responseMessage = await response.Content.ReadAsStringAsync();

                    if (SaveResponseToDynamics(serviceClient, contactId, responseMessage, out errorMessage) == Guid.Empty)
                    {
                        return new ScreeningApiResult
                        {
                            IsSuccessful = false,
                            Message = errorMessage
                        };
                    }
                    return new ScreeningApiResult
                    {
                        IsSuccessful = true,
                        Message = "eKYC check successfully gone through D365. Please check Identity Verification table for detail"
                    };
                }
                else
                {
                    return new ScreeningApiResult
                    {
                        IsSuccessful = false,
                        Message = errorMessage
                    };
                }
            }
            catch (Exception ex)
            {
                return new ScreeningApiResult
                {
                    IsSuccessful = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// SaveResponseToDynamics.
        /// </summary>
        /// <param name="serviceClient"></param>
        /// <param name="contactId"></param>
        /// <param name="responseMessage"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public Guid SaveResponseToDynamics(IOrganizationServiceAsync2 serviceClient, string contactId, string responseMessage, out string errorMessage)
        {
            try
            {
                int? riskScore = 0;
                string status = "Fail";
                var jsonResponse = JsonSerializer.Deserialize<LexisNexisUSResponse>(responseMessage);
                riskScore = jsonResponse?.InstantIDResponseEx?.response?.Result?.ComprehensiveVerification?.ComprehensiveVerificationIndex;
                var watchlistErrors = jsonResponse?.InstantIDResponseEx?.response?.Result?.ComprehensiveVerification?.RiskIndicators?.RiskIndicator?.Where(r => r.RiskCode == "WL" || r.RiskCode == "32");
                var otherReferErrors = jsonResponse?.InstantIDResponseEx?.response?.Result?.ComprehensiveVerification?.RiskIndicators?.RiskIndicator?.Where(r => LexNexConstants.referRiskIndicators.Contains(r.RiskCode)) ?? Enumerable.Empty<RiskIndicator>();

                if (watchlistErrors?.Count() > 0 || otherReferErrors.Count() > 0)
                    status = "Refer";

                if (riskScore >= 40)
                    status = "Pass";

                var decisionText = watchlistErrors.Any() ? jsonResponse?.InstantIDResponseEx?.response?.Result?.WatchLists?.WatchList?[0].Table : "";

                Entity identityVerification = new Entity("mcorp_identityverification");
                identityVerification["mcorp_name"] = status;
                identityVerification["mcorp_decisiondetail"] = responseMessage;
                identityVerification["mcorp_riskscores"] = riskScore;
                identityVerification["mcorp_decisiontext"] = decisionText;
                identityVerification["mcorp_contact"] = new EntityReference("contact", Guid.Parse(contactId));

                // Save the record to Dynamics 365
                Guid idVerficiation = serviceClient.Create(identityVerification);
                errorMessage = string.Empty;
                return idVerficiation;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return Guid.Empty;
            }
        }
    }
}
