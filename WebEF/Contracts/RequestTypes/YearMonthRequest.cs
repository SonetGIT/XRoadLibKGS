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
    public class YearMonthRequest : XRoadSerializable, Domain.RequestTypes.IYearMonthRequest
    {
        [XRoadXmlElement(Order = 1)]
        public int Year { get; set; }
        [XRoadXmlElement(Order = 2)]
        public int Month { get; set; }
    }
}
