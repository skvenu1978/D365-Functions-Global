using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Models.Ekyc.Experian
{
    [ExcludeFromCodeCoverage]
    public class ExperianCCRequest
    {
        public Header? header { get; set; }
        public Payload? payload { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Address
    {
        public string? id { get; set; }
        public string? addressIdentifier { get; set; }
        public string? indicator { get; set; }
        public string? addressType { get; set; }
        public object? subBuilding { get; set; }
        public string? buildingNumber { get; set; }
        public object? buildingName { get; set; }
        public string? street { get; set; }
        public string? postTown { get; set; }
        public string? county { get; set; }
        public string? postal { get; set; }
        public string? countryCode { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Applicant
    {
        public string? id { get; set; }
        public object? contactId { get; set; }
        public string? type { get; set; }
        public string? applicantType { get; set; }
        public bool? consent { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Application
    {
        public List<Applicant>? applicants { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Contact
    {
        public string? id { get; set; }
        public Person? person { get; set; }
        public List<Address>? addresses { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Header
    {
        public string? tenantID { get; set; }
        public string? clientReferenceId { get; set; }
        public string? requestType { get; set; }
        public string messageTime { get; set; }
        public string? expRequestId { get; set; }
        public Options? options { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Name
    {
        public string? id { get; set; }
        public string? type { get; set; }
        public string? title { get; set; }
        public string? firstName { get; set; }
        public string? surName { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Options
    {
    }

    [ExcludeFromCodeCoverage]
    public class Payload
    {
        public string? source { get; set; }
        public List<Contact>? contacts { get; set; }
        public Application? application { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class Person
    {
        public PersonDetails? personDetails { get; set; }
        public string? typeOfPerson { get; set; }
        public List<Name>? names { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class PersonDetails
    {
        public string? dateOfBirth { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AuthToken
    {

        public string? access_token { get; set; }
    }
}
