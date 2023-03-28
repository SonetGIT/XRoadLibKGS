using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XRoadLib.Serialization;

namespace PortableClassLibrary.RequestTypes
{
    public class YearMonthRequest : XRoadSerializable
    {
        [XmlElement(Order = 1)]
        public int Year { get; set; }
        [XmlElement(Order = 2)]
        public int Month { get; set; }
    }
}
