using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.BalagaSuyunchu
{
    public class BirthPaymentApplication : XRoadSerializable
    {
        [XRoadXmlElement(Order = 1)]
        public DateTime? DocDate { get; set; }
        [XRoadXmlElement(Order = 2)]
        public string DocNumber { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string GovUnit { get; set; }
        [XRoadXmlElement(Order = 4)]
        public int GovUnitId { get; set; }
        [XRoadXmlElement(Order = 5)]
        public int ApplicantRole { get; set; }
        [XRoadXmlElement(Order = 6)]
        public Applicant Applicant { get; set; }
        [XRoadXmlElement(Order = 7)]
        public int? AsbStreetId { get; set; }
        [XRoadXmlElement(Order = 8)]
        public int? AsbAteId { get; set; }
        [XRoadXmlElement(Order = 9)]
        public string AsbHouse { get; set; }
        [XRoadXmlElement(Order = 10)]
        public string AsbFlat { get; set; }
        [XRoadXmlElement(Order = 11)]
        public string AsbText { get; set; }
        [XRoadXmlElement(Order = 12)]
        public DateTime AsbDate { get; set; }
        [XRoadXmlElement(Order = 16)]
        public string AddressGrnp { get; set; }
        [XRoadXmlElement(Order = 17)]
        public string TelHome { get; set; }
        [XRoadXmlElement(Order = 18)]
        public string TelMob { get; set; }
        [XRoadXmlElement(Order = 19)]
        public string TelWork { get; set; }
        [XRoadXmlElement(Order = 20)]
        public string Email { get; set; }
        [XRoadXmlElement(Order = 21)]
        public int Bank { get; set; }
        [XRoadXmlElement(Order = 22)]
        public string BankAccount { get; set; }
        [XRoadXmlElement(Order = 24)]
        public Child Child { get; set; }
        [XRoadXmlElement(Order = 26, IsOptional = true)]
        public Father Father { get; set; }
        [XRoadXmlElement(Order = 24, IsOptional = true)]
        public Mother Mother { get; set; }
        [XRoadXmlElement(Order = 29)]
        public string serverType { get; set; }
    }
    public class BirthPaymentAssignment : XRoadSerializable
    {
        [XRoadXmlElement(Order = 1, IsOptional = true)]
        public int Id { get; set; }
        [XRoadXmlElement(Order = 2)]
        public int? BirthPaymentApplicationId { get; set; }
        [XRoadXmlElement(Order = 3)]
        public decimal PaymentSum { get; set; } = 0;
    }

    public class Applicant : XRoadSerializable
    {
        [XRoadXmlElement(Order = 1, IsOptional = true)]
        public int Id { get; set; }
        [XRoadXmlElement(Order = 2)]
        public string PIN { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string LastName { get; set; }
        [XRoadXmlElement(Order = 4)]
        public string FirstName { get; set; }
        [XRoadXmlElement(Order = 5)]
        public string MiddleName { get; set; }
        [XRoadXmlElement(Order = 6)]
        public DateTime BirthDate { get; set; }
        [XRoadXmlElement(Order = 7)]
        public int Gender { get; set; }
        [XRoadXmlElement(Order = 8)]
        public int DocumentType { get; set; }
        [XRoadXmlElement(Order = 9)]
        public string DocumentSeries { get; set; }
        [XRoadXmlElement(Order = 10)]
        public string DocumentNo { get; set; }
        [XRoadXmlElement(Order = 11)]
        public DateTime DocumentIssuedDate { get; set; }
        [XRoadXmlElement(Order = 12)]
        public string DocumentIssuedByOrg { get; set; }
        [XRoadXmlElement(Order = 13)]
        public DateTime? DocumentValidTo { get; set; }
    }
    
    public class Child : XRoadSerializable
    {
        [XRoadXmlElement(Order = 1, IsOptional = true)]
        public int Id { get; set; }
        [XRoadXmlElement(Order = 2)]
        public string PIN { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string LastName { get; set; }
        [XRoadXmlElement(Order = 4)]
        public string FirstName { get; set; }
        [XRoadXmlElement(Order = 5)]
        public string MiddleName { get; set; }
        [XRoadXmlElement(Order = 6)]
        public DateTime BirthDate { get; set; }
        [XRoadXmlElement(Order = 7)]
        public int GenderId { get; set; }
        [XRoadXmlElement(Order = 8)]
        public string BirthCertificateSeries { get; set; }
        [XRoadXmlElement(Order = 9)]
        public string BirthCertificateNo { get; set; }
        [XRoadXmlElement(Order = 10)]
        public DateTime BirthCertificateDate { get; set; }
        [XRoadXmlElement(Order = 11)]
        public string BirthCertificateIssuedByOrg { get; set; }
        [XRoadXmlElement(Order = 12)]
        public DateTime? BirthActDate { get; set; }
        [XRoadXmlElement(Order = 13)]
        public string BirthActNo { get; set; }
        [XRoadXmlElement(Order = 14)]
        public string BirthActGovUnit { get; set; }
    }
    
    public class Father : XRoadSerializable
    {
        [XRoadXmlElement(Order = 1, IsOptional = true)]
        public int Id { get; set; }
        [XRoadXmlElement(Order = 2)]
        public string PIN { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string LastName { get; set; }
        [XRoadXmlElement(Order = 4)]
        public string FirstName { get; set; }
        [XRoadXmlElement(Order = 5)]
        public string MiddleName { get; set; }
        [XRoadXmlElement(Order = 6)]
        public DateTime BirthDate { get; set; }
    }
    
    public class Mother : XRoadSerializable
    {
        [XRoadXmlElement(Order = 1, IsOptional = true)]
        public int Id { get; set; }
        [XRoadXmlElement(Order = 2)]
        public string PIN { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string LastName { get; set; }
        [XRoadXmlElement(Order = 4)]
        public string FirstName { get; set; }
        [XRoadXmlElement(Order = 5)]
        public string MiddleName { get; set; }
        [XRoadXmlElement(Order = 6)]
        public DateTime BirthDate { get; set; }
    }
}
