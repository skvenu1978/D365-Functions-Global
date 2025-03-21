using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace McGlobalAzureFunctions.Models.Ekyc
{
    public class LexisNexisRequest
    {
        public string? Type { get; set; }
        public Settings? Settings { get; set; }
        public List<Person>? Persons { get; set; }
    }

    public class Address
    {
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? Locality { get; set; }
        public string? Province { get; set; }
        public string? Postcode { get; set; }
        public string? Country { get; set; }
        public string? Context { get; set; }
    }

    public class DateOfBirth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }

    public class Name
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
    }

    public class Person
    {
        public Name? Name { get; set; }
        public List<Address>? Addresses { get; set; }
        public DateOfBirth? DateOfBirth { get; set; }
        public List<Phone>? Phones { get; set; }
        public string? Context { get; set; }
    }

    public class Phone
    {
        public string? Number { get; set; }
        public string? Context { get; set; }
    }

    public class Settings
    {
        public string? Mode { get; set; }
        public string? Reference { get; set; }
        public string? Locale { get; set; }
        public string? Venue { get; set; }
    }
    public class AuthToken { 
    
        public string? access_token { get; set; }
    }
}
