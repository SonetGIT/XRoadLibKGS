using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class GetActivePaymentsByPINResponse : XRoadSerializable, Domain.ResponseTypes.IGetActivePaymentsByPINResponse
    {
        [XRoadXmlElement(Order = 1)]
        public DateTime StartDate { get; set; }
        [XRoadXmlElement(Order = 2)]
        public DateTime EndDate { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string PaymentTypeName { get; set; }
        [XRoadXmlElement(Order = 4)]
        public decimal PaymentSize { get; set; }
        [XRoadXmlElement(Order = 5)]
        public string OrganizationName { get; set; }
        /*[XRoadXmlElement(Order = 6)]
        public string LastName { get; set; }
        [XRoadXmlElement(Order = 7)]
        public string FirstName { get; set; }
        [XRoadXmlElement(Order = 8)]
        public string MiddleName { get; set; }*/
        [XRoadXmlElement(Order = 9)]
        public bool IsActive
        {
            get; set;
        }
        [XRoadXmlArray(Order = 10)]
        public string[] Dependants { get; set; }
    }
}
