using Domain.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class SetPaymentInfoResponse : XRoadSerializable, ISetPaymentInfoResponse
    {
        [XRoadXmlElement(Order = 1)]
        public bool Result { get; set; }
    }
}
