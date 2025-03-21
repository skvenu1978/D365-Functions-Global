using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Models.Ekyc
{
    public class IDnVRequest
    {
        public string? AccountGuid { get; set; }
        public string? ContactGuid { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? StateOrCounty { get; set; }
        public string? ZipOrPostCode { get; set; }
        public string? Country { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PCAKey { get; set; }
        public int CustomerId { get; set; }
        public int ApplicationSourceId { get; set; }
        public int ClientTypeId { get; set; }
    }
    public class LexisNexisUSRequest
    {
        public class User
        {
            public string? ReferenceCode { get; set; }
            public string? BillingCode { get; set; }
            public string? QueryId { get; set; }
            public string? GLBPurpose { get; set; }
            public string? DLPurpose { get; set; }
        }

        public class Name
        {
            public string? First { get; set; }
            public string? Last { get; set; }
        }

        public class Address
        {
            public string? StreetAddress1 { get; set; }
            public string? StreetAddress2 { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? Zip5 { get; set; }
        }

        public class DOB
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
        }

        public class SearchBy
        {
            public Name? Name { get; set; }
            public Address? Address { get; set; }
            public DOB? DOB { get; set; }
            public string? SSN { get; set; }
            public string? HomePhone { get; set; }
        }
        public class DOBMatch
        {
            public string? MatchType { get; set; }
        }

        public class FraudPointModel
        {
            public bool IncludeRiskIndices { get; set; }
        }

        public class IncludeModels
        {
            public FraudPointModel? FraudPointModel { get; set; }
        }

        public class Options
        {
            public List<WatchList>? WatchLists { get; set; }
            public IncludeModels IncludeModels { get; set; }
            public DOBMatch DOBMatch { get; set; }
        }

        public class InstantIdRequest
        {
            public User? User { get; set; }
            public Options? Options { get; set; }
            public SearchBy? SearchBy { get; set; }
        }

        public class RequestWrapper
        {
            public InstantIdRequest? InstantIdRequest { get; set; }
        }

        private static IEnumerable<WatchList> watchlists = new List<WatchList>
            {
                new WatchList { watchList = "FBI" },
                new WatchList { watchList = "OFAC" }

            };
        public class WatchList
        {
            public string? watchList { get; set; }

        }
        public static string ToJson(IDnVRequest request)
        {
            var instantRequest = new RequestWrapper
            {
                InstantIdRequest = new InstantIdRequest
                {
                    User = new User
                    {
                        ReferenceCode = "RefCode",
                        BillingCode = "BillingCode",
                        QueryId = request.ContactGuid,
                        GLBPurpose = "5",
                        DLPurpose = "3"
                    },
                    Options = new Options
                    {
                        //WatchLists = new List<WatchList> { new WatchList { watchList="FBI"} },
                        DOBMatch = new DOBMatch { MatchType = "FuzzyCCYYMMDD" },
                        IncludeModels = new IncludeModels { FraudPointModel = new FraudPointModel { IncludeRiskIndices = true } }
                    },
                    SearchBy = new SearchBy
                    {
                        Name = new Name
                        {
                            First = request.FirstName,
                            Last = request.LastName
                        },
                        Address = new Address
                        {
                            StreetAddress1 = request.AddressLine1,
                            StreetAddress2 = request.AddressLine2,
                            City = request.City,
                            State = request.StateOrCounty,
                            Zip5 = request.ZipOrPostCode
                        },
                        DOB = new DOB
                        {
                            Year = request.DateOfBirth.Year,
                            Month = request.DateOfBirth.Month,
                            Day = request.DateOfBirth.Day
                        },
                        SSN = request.NationalId?.Replace("-", ""),
                        HomePhone = request.PhoneNumber
                    }
                }
            };

            return JsonConvert.SerializeObject(instantRequest);
        }
    }

}
