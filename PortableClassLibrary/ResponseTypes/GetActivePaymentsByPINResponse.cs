using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XRoadLib.Serialization;

namespace PortableClassLibrary.ResponseTypes
{
    public class GetActivePaymentsByPINResponse : XRoadSerializable
    {
        [XmlElement(Order = 1)]
        public DateTime StartDate { get; set; }
        [XmlElement(Order = 2)]
        public DateTime EndDate { get; set; }
    }
}
