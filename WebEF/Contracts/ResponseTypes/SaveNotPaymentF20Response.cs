using Domain.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class SaveNotPaymentF20Response : XRoadSerializable, ISaveNotPaymentF20Response
    {
        [XRoadXmlElement(Order = 1)]
        public string PaymentF20Id { get; set; }
    }
}