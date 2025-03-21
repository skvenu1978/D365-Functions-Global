// <copyright file="LexisNexisProvider.cs" company="moneycorp">
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
    using System.Collections.Generic;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation for ILexisNexisProvider
    /// </summary>
    public class LexisNexisProvider : ILexisNexisProvider
    {
        /// <summary>
        /// Create Request object.
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="serviceClient"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public LexisNexisRequest? CreateRequest(string contactId, IOrganizationServiceAsync2 serviceClient, out string errorMessage)
        {
            try
            {
                Entity contact = serviceClient.Retrieve("contact", Guid.Parse(contactId), new ColumnSet(true));

                var name = new Name
                {
                    FirstName = contact[LexNexConstants.att_firstName].ToString(),
                    LastName = contact[LexNexConstants.att_lastname].ToString()
                };

                var addressList = new List<Address>();

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

                    addressList.Add(new Address()
                    {
                        StreetAddress1 = entity.Contains(LexNexConstants.att_buildingNumber) ? entity[LexNexConstants.att_buildingNumber].ToString() : "",
                        StreetAddress2 = entity.Contains(LexNexConstants.att_street) ? entity[LexNexConstants.att_street].ToString() : "",
                        Locality = entity.Contains(LexNexConstants.att_city) ? entity[LexNexConstants.att_city].ToString() : "",
                        Postcode = entity.Contains(LexNexConstants.att_postal) ? entity[LexNexConstants.att_postal].ToString() : "",
                        Province = entity.Contains(LexNexConstants.att_county) ? entity[LexNexConstants.att_county].ToString() : "",
                        Country = country.Contains(LexNexConstants.att_countrycode) ? country[LexNexConstants.att_countrycode].ToString() : "",
                        Context = "primary"
                    });
                }

                var phone = new Phone
                {
                    Number = contact.Contains(LexNexConstants.att_phone) ? contact[LexNexConstants.att_phone].ToString() : "",
                    Context = "mobile"
                };

                var dob = new DateOfBirth();

                if (contact.Contains(LexNexConstants.att_dob))
                {
                    dob.Year = contact.GetAttributeValue<DateTime>(LexNexConstants.att_dob).Year;
                    dob.Month = contact.GetAttributeValue<DateTime>(LexNexConstants.att_dob).Month;
                    dob.Day = contact.GetAttributeValue<DateTime>(LexNexConstants.att_dob).Day;
                }

                var person = new Person
                {
                    Name = name,
                    Addresses = addressList,
                    DateOfBirth = dob,
                    Phones = new List<Phone>() { phone },
                    Context = "primary"
                };

                var persons = new List<Person>();
                persons.Add(person);

                var lexisNexisRequest = new LexisNexisRequest()
                {
                    Type = "Initiate",
                    Settings = new Settings
                    {
                        Mode = "testing",
                        Locale = "en_US",
                        Venue = "online"
                    },
                    Persons = persons,
                };

                errorMessage = string.Empty;

                return lexisNexisRequest;
            }
            catch (Exception)
            {
                errorMessage = "Error in Request";
                return null;
            }
        }

        /// <summary>
        /// GetToken.
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        public async Task<string>? GetToken(string? endpointUrl, string? clientId, string? clientSecret)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl);
                var authenticationString = $"{clientId}:{clientSecret}";
                var base64String = Convert.ToBase64String(
                   Encoding.ASCII.GetBytes(authenticationString));

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("grant_type", "client_credentials"));
                collection.Add(new("scope", "write"));
                var content = new FormUrlEncodedContent(collection);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseMessage = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonSerializer.Deserialize<AuthToken>(responseMessage);

                return jsonResponse?.access_token;

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// GetScreeningRecord.
        /// </summary>
        /// <param name="base64String"></param>
        /// <param name="restURL"></param>
        /// <param name="contactId"></param>
        /// <param name="serviceClient"></param>
        /// <returns></returns>
        public async Task<ScreeningApiResult> GetScreeningRecord(string base64String, string restURL, string contactId, IOrganizationServiceAsync2 serviceClient)
        {
            try
            {
                string errorMessage = string.Empty;
                var requestPayload = CreateRequest(contactId, serviceClient, out errorMessage);

                if (requestPayload != null)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, restURL);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
                    var requestConent = JsonSerializer.Serialize(requestPayload);
                    var content = new StringContent(requestConent, null, "application/json");

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
                return new ScreeningApiResult
                {
                    IsSuccessful = false,
                    Message = errorMessage
                };
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
        /// Saves Response to D365.
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
                var jsonResponse = JsonSerializer.Deserialize<LexisNexisResponse>(responseMessage);
                int riskScore = MapToRiskScore(jsonResponse?.Status?.TransactionStatus);
                var decisionText = jsonResponse?.Products?[0].Items?[0].ItemReason?.Code;
                Entity identityVerification = new Entity("mcorp_identityverification");
                identityVerification["mcorp_name"] = jsonResponse?.Status?.TransactionStatus;
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

        /// <summary>
        /// MapToRiskScore.
        /// </summary>
        /// <param name="score">score.</param>
        /// <returns></returns>
        private int MapToRiskScore(string? score)
        {
            switch (score)
            {
                case "passed":
                    return 40;
                case "error":
                    return 0;
                default:
                    return 30;
            }
        }
    }
}
