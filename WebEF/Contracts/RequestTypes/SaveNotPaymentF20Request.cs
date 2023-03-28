using Domain.RequestTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.RequestTypes
{
    public class SaveNotPaymentF20Request : XRoadSerializable, ISaveNotPaymentF20Request
    {
        [XRoadXmlElement(Order = 1)]
        public string OrderPaymentId { get; set; }
        [XRoadXmlElement(Order = 2)]
        public DateTime RegDate { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string ReasonId { get; set; }
    }
}