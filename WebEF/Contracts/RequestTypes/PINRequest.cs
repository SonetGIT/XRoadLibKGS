using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.RequestTypes
{
    public class PINRequest : XRoadSerializable, Domain.RequestTypes.IPINRequest
    {
        [XRoadXmlElement(Order = 1)]
        public string PIN { get; set; }
    }
}
