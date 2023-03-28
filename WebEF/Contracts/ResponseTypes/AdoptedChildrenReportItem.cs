using Domain.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class AdoptedChildrenReportItem : XRoadSerializable, IAdoptedChildrenReportItem
    {
        [XRoadXmlElement(Order = 1)]
        public int No { get; set; }
        [XRoadXmlElement(Order = 2)]
        public string Name { get; set; }
        [XRoadXmlElement(Order = 3)]
        public int Boys { get; set; }
        [XRoadXmlElement(Order = 4)]
        public int Girls { get; set; }
        [XRoadXmlElement(Order = 5)]
        public int Total { get; set; }
    }
}
