﻿using System.Xml.Serialization;
using XRoadLib.Serialization;

namespace xRoadWeb.Contracts
{
    public class CalculationRequest : XRoadSerializable
    {
        [XmlElement(Order = 1)]
        public int X { get; set; }

        [XmlElement(Order = 2)]
        public int Y { get; set; }

        [XmlElement(Order = 3)]
        public Operation Operation { get; set; }
    }
}