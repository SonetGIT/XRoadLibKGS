using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XRoadLib.Serialization;

namespace PortableClassLibrary.RequestTypes
{
    public class PINRequest : XRoadSerializable
    {
        [XmlElement(Order = 1)]
        public string PIN { get; set; }
    }
}
