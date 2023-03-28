using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XRoadLib.Serialization;

namespace PortableClassLibrary.ResponseTypes
{
    public class GetNewOldRecipientsByYearMonthResponse : XRoadSerializable
    {
        [XmlElement(Order = 1)]
        public int TotalNew
        {
            get
            {
                return NewPINs != null ? NewPINs.Length : 0;
            }
        }
        [XmlElement(Order = 2)]
        public int TotalExpired
        {
            get
            {
                return ExpiredPINs != null ? ExpiredPINs.Length : 0;
            }
        }
        [XmlElement(Order = 3)]
        public string[] NewPINs { get; set; }
        [XmlElement(Order = 4)]
        public string[] ExpiredPINs { get; set; }
    }
}
