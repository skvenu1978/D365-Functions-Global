// <copyright file="ExperianProvider.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.DAL.Ekyc
{
    using McGlobalAzureFunctions.Abstractions.Ekyc;
    using McGlobalAzureFunctions.Const;
    using McGlobalAzureFunctions.Models.Ekyc.Experian;
    using Microsoft.PowerPlatform.Dataverse.Client;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation for IExperianProvider.
    /// </summary>
    public class ExperianProvider : IExperianProvider
    {
        public async Task<string?> GetToken(string authUrl, string bodyContent)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, authUrl);
                request.Headers.Add("X-Correlation-Id", "00000000-0000-0000-0000-000000000000");
                request.Headers.Add("X-User-Domain", "moneycorp.com");
                var content = new StringContent(bodyContent, new MediaTypeHeaderValue("application/json"));
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseMessage = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<AuthToken>(responseMessage);

                if (jsonResponse?.access_token != null)
                {
                    return jsonResponse?.access_token;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public ExperianCCRequest? BuildCCRequest(string contactId, IOrganizationServiceAsync2 serviceClient, out string errorMessage)
        {
            try
            {
                Entity contactEntity = serviceClient.Retrieve("contact", Guid.Parse(contactId), new ColumnSet(true));
                var name = new Name
                {
                    firstName = contactEntity[CrossCoreConstants.att_firstName].ToString(),
                    surName = contactEntity[CrossCoreConstants.att_lastname].ToString(),
                    type = "CURRENT"
                };

                var addressList = new List<Address>();

                QueryExpression query = new QueryExpression("mcorp_moneycorpaddress")
                {
                    ColumnSet = new ColumnSet(true)
                };

                query.Criteria.AddCondition(new ConditionExpression("mcorp_parentid", ConditionOperator.Equal, contactEntity.Id));
                query.Criteria.AddCondition(new ConditionExpression("mcorp_addresstype", ConditionOperator.Equal, 1));
                EntityCollection results = serviceClient.RetrieveMultiple(query);

                foreach (Entity entity in results.Entities)
                {
                    addressList.Add(new Address()
                    {
                        addressType = "CURRENT",
                        buildingNumber = entity.Contains(CrossCoreConstants.att_buildingNumber) ? entity[CrossCoreConstants.att_buildingNumber].ToString() : "",
                        street = entity.Contains(CrossCoreConstants.att_street) ? entity[CrossCoreConstants.att_street].ToString() : "",
                        postTown = entity.Contains(CrossCoreConstants.att_posttown) ? entity[CrossCoreConstants.att_posttown].ToString() : "",
                        county = entity.Contains(CrossCoreConstants.att_county) ? entity[CrossCoreConstants.att_county].ToString() : "",
                        postal = entity.Contains(CrossCoreConstants.att_postal) ? entity[CrossCoreConstants.att_postal].ToString() : "",
                        indicator = "RESIDENTIAL",
                        countryCode = "GBR"
                    });
                }

                var applicant = new Applicant
                {
                    id = "APPLICANT_1",
                    contactId = contactId,
                    type = "INDIVIDUAL",
                    applicantType = "APPLICANT",
                    consent = true
                };
                var person = new Person
                {
                    personDetails = new PersonDetails
                    {
                        dateOfBirth = contactEntity.Contains("birthdate") ? contactEntity.GetAttributeValue<DateTime>("birthdate").ToString("yyyy-MM-dd") : ""
                    },
                    typeOfPerson = "APPLICANT",
                    names = new List<Name>() { name }
                };
                var contact = new Contact
                {
                    addresses = addressList,
                    id = contactId,
                    person = person,

                };
                var payload = new Payload
                {
                    source = "WEB",
                    contacts = new List<Contact>() { contact },
                    application = new Application()
                    {
                        applicants = new List<Applicant>() { applicant }
                    }
                };

                string messageTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                var requestPayload = new ExperianCCRequest
                {
                    header = new Header
                    {
                        messageTime = messageTime,
                        options = new Options(),
                        clientReferenceId = contactId,
                        requestType = "Authenticateplus-Standalone",
                        expRequestId = null,
                        tenantID = CrossCoreConstants.tenantId
                    },
                    payload = payload
                };
                errorMessage = string.Empty;
                return requestPayload;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }

        public async Task<bool> DoeKYCCheck(string token, string contactId, IOrganizationServiceAsync2 serviceClient, string sceeningApi)
        {
            try
            {
                string errorMessage = string.Empty;
                var requestContent = BuildCCRequest(contactId, serviceClient, out errorMessage);
                if (requestContent != null)
                {
                    var jsonContent = JsonSerializer.Serialize(requestContent);
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, sceeningApi);
                    request.Headers.Add("Authorization", "Bearer " + token);

                    var content = new StringContent(jsonContent, new MediaTypeHeaderValue("application/json"));
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseMessage = await response.Content.ReadAsStringAsync();
                    if (SaveResponseToDynamics(serviceClient, contactId, responseMessage) == Guid.Empty)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public Guid SaveResponseToDynamics(IOrganizationServiceAsync2 serviceClient, string contactId, string responseMessage)
        {

            try
            {
                var jsonResponse = JsonSerializer.Deserialize<ExperianCCResponse>(responseMessage);
                if (jsonResponse != null)
                {
                    var crmResult = GetCrmExperianResult(jsonResponse);

                    Entity identityVerification = new Entity("mcorp_identityverification");
                    identityVerification["mcorp_name"] = crmResult.Decision;
                    identityVerification["mcorp_decisiondetail"] = responseMessage.Length > 5000 ? responseMessage.Substring(0, 5000) : responseMessage;
                    identityVerification["mcorp_riskscores"] = crmResult.DecisionScore;
                    identityVerification["mcorp_decisiontext"] = crmResult.DecisionText;
                    identityVerification["mcorp_contact"] = new EntityReference("contact", Guid.Parse(contactId));

                    // Save the record to Dynamics 365
                    Guid idVerficiation = serviceClient.Create(identityVerification);
                    return idVerficiation;
                }
                else
                {
                    return Guid.Empty;
                }
            }
            catch (Exception ex)
            {

                return Guid.Empty;
            }
        }

        public CrmEKYCResult GetCrmExperianResult(ExperianCCResponse experianResponse)
        {
            CrmEKYCResult result = new CrmEKYCResult();

            if (experianResponse != null && experianResponse.clientResponsePayload != null)
            {
                if (experianResponse?.clientResponsePayload?.orchestrationDecisions != null && experianResponse.clientResponsePayload.orchestrationDecisions.Count > 0)
                {
                    OrchestrationDecision dec = experianResponse.clientResponsePayload.orchestrationDecisions[0];
                    result.DecisionText = dec.decisionText;
                    result.Decision = dec.decision;
                }

                if (experianResponse?.clientResponsePayload?.decisionElements != null && experianResponse?.clientResponsePayload?.decisionElements?.Count > 0)
                {
                    DecisionElement dec = experianResponse.clientResponsePayload.decisionElements[0];
                    result.AppReference = dec.appReference;
                    result.DecisionCode = dec.decision;
                    result.DecisionReason = dec.decisionReason;

                    if (dec.score != int.MinValue)
                        result.DecisionScore = dec.score;
                }
            }
            else if (experianResponse != null && experianResponse.clientResponsePayload == null && experianResponse.responseHeader != null)
            {
                string _decisionReason = string.Empty;
                if (!string.IsNullOrEmpty(experianResponse.responseHeader?.responseType) && experianResponse.responseHeader.responseType.ToString().ToUpper() == "ERROR")
                    result.Decision = experianResponse.responseHeader.responseType.ToString();

                if (!string.IsNullOrEmpty(experianResponse?.responseHeader?.responseCode))
                    _decisionReason = experianResponse.responseHeader.responseCode.ToString().Length > 510 // Max length of DecisionDetail field.
                      ? experianResponse.responseHeader.responseCode.ToString().Substring(0, 500) + " : "
                      : experianResponse.responseHeader.responseCode.ToString() + " : ";

                if (!string.IsNullOrEmpty(experianResponse?.responseHeader?.responseMessage))
                    _decisionReason = experianResponse.responseHeader.responseMessage.ToString().Length > 510 // Max length of DecisionDetail field.
                  ? _decisionReason + experianResponse.responseHeader.responseMessage.ToString().Substring(0, 500) + "..."
                  : _decisionReason + experianResponse.responseHeader.responseMessage.ToString();

                result.DecisionReason = _decisionReason;
            }

            if (experianResponse != null && experianResponse.responseHeader != null && !string.IsNullOrEmpty(experianResponse?.responseHeader?.expRequestId))
            {
                result.ExpRequestId = experianResponse.responseHeader.expRequestId;
            }

            return result;
        }
    }
}
