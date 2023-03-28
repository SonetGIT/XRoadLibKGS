using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class BalagaSuyunchuResponse : XRoadSerializable
    {
        [XRoadXmlElement(Order = 1)]
        public bool result { get; set; }
        [XRoadXmlElement(Order = 2)]
        public _data data { get; set; }
        [XRoadXmlElement(Order = 3)]
        public string errorMessage { get; set; }
        public class _data : XRoadSerializable
        {
            [XRoadXmlElement(Order = 1)]
            public int Id { get; set; }
        }
    }
}
