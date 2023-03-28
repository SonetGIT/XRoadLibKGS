﻿using System.Xml.Serialization;
using XRoadLib.Attributes;

namespace xRoadWeb.Contracts
{
    [XmlType]
    [XRoadTitle("en", "Operation type")]
    [XRoadNotes("en", "Defines operations to perform on given arguments")]
    public enum Operation
    {
        [XRoadTitle("en", "Add")]
        Add,

        [XRoadTitle("en", "Subtract")]
        Subtract,

        [XRoadTitle("en", "Multiply")]
        Multiply,

        [XRoadTitle("en", "Divide")]
        Divide
    }
}