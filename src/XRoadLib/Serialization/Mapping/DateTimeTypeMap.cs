﻿using System;
using System.Xml;
using XRoadLib.Schema;
using XRoadLib.Serialization.Template;

namespace XRoadLib.Serialization.Mapping
{
    public class DateTimeTypeMap : TypeMap
    {
        public DateTimeTypeMap(TypeDefinition typeDefinition)
            : base(typeDefinition)
        { }

        public override object Deserialize(XmlReader reader, IXmlTemplateNode templateNode, ContentDefinition content, XRoadMessage message)
        {
            if (reader.IsEmptyElement)
                return MoveNextAndReturn(reader, new DateTime());

            var value = reader.ReadElementContentAsString();
            if (string.IsNullOrEmpty(value))
                return null;

            var dateTime = XmlConvert.ToDateTimeOffset(value).DateTime;

            return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));
        }

        public override void Serialize(XmlWriter writer, IXmlTemplateNode templateNode, object value, ContentDefinition content, XRoadMessage message)
        {
            message.Style.WriteType(writer, Definition, content);

            writer.WriteValue(value);
        }
    }
}