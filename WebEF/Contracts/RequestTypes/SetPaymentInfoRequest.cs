using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.RequestTypes
{
    public class SetPaymentInfoRequest : XRoadSerializable, Domain.RequestTypes.ISetPaymentInfoRequest
    {
        [XRoadXmlElement(Order = 1)]
        public string PIN { get; set; }
        [XRoadXmlElement(Order = 2)]
        public decimal Amount { get; set; }
        [XRoadXmlElement(Order = 3)]
        public DateTime PayDate { get; set; }
    }
}
