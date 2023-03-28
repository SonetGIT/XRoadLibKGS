using Domain.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class SavePaymentF10Response : XRoadSerializable, ISavePaymentF10Response
    {
        [XRoadXmlElement(Order = 1)]
        public string PaymentF10Id { get; set; }
    }
}
