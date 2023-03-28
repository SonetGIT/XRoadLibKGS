using System.Xml.Serialization;
using XRoadLib.Serialization;

namespace xRoadWeb.Contracts
{
    public class AddRequest : XRoadSerializable
    {
        [XmlElement(Order = 1)]
        public int X { get; set; }

        [XmlElement(Order = 2)]
        public int Y { get; set; }
    }
}