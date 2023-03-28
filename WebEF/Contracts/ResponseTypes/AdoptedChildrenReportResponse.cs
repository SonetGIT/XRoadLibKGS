using Domain.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class AdoptedChildrenReportResponse : XRoadSerializable
    {
        [XRoadXmlArray(Order = 1)]
        public AdoptedChildrenReportItem[] ByAge { get; set; }
        [XRoadXmlArray(Order = 2)]
        public AdoptedChildrenReportItem[] ByNationalities { get; set; }
        [XRoadXmlArray(Order = 3)]
        public AdoptedChildrenReportItem[] ByGeography { get; set; }
    }
}
