using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class MSECDetailsResponse : XRoadSerializable, Domain.ResponseTypes.IMSECDetails
    {
        [XRoadXmlElement(Order = 1)]
        public string OrganizationName { get; set; }
        [XRoadXmlElement(Order = 2)]
        public DateTime ExaminationDate { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string ExaminationType { get; set; }
        [XRoadXmlElement(Order = 4)]
        public string DisabilityGroup { get; set; }
        [XRoadXmlElement(Order = 5)]
        public DateTime From { get; set; }
        [XRoadXmlElement(Order = 6)]
        public DateTime To { get; set; }
        [XRoadXmlElement(Order = 7)]
        public DateTime TimeOfDisability { get; set; }
        [XRoadXmlElement(Order = 8)]
        public string ReExamination { get; set; }
        [XRoadXmlElement(Order = 9)]
        public string StatusCode { get; set; }
    }
}
