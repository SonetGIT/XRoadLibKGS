﻿using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using XRoadLib.Serialization;

namespace XRoadLib.Extensions
{
    /// <summary>
    /// Extension methods for XmlReader class.
    /// </summary>
    public static class XmlReaderExtensions
    {
        private static readonly ICollection<string> headerNamespaces = new[]
        {
            NamespaceConstants.XTEE,
            NamespaceConstants.XROAD,
            NamespaceConstants.XROAD_V4,
            NamespaceConstants.XROAD_V4_REPR
        };

        private static readonly XName qnXsiNil = XName.Get("nil", NamespaceConstants.XSI);
        private static readonly XName qnXsiType = XName.Get("type", NamespaceConstants.XSI);
        private static readonly XName qnSoapEncArray = XName.Get("Array", NamespaceConstants.SOAP_ENC);
        private static readonly XName qnSoapEncArrayType = XName.Get("arrayType", NamespaceConstants.SOAP_ENC);

        /// <summary>
        /// Test if current element is marked as nil with xsi attribute.
        /// </summary>
        public static bool IsNilElement(this XmlReader reader)
        {
            var value = reader.GetAttribute(qnXsiNil.LocalName, qnXsiNil.NamespaceName);

            switch (value)
            {
                case "1":
                case "true":
                    return true;

                case "0":
                case "false":
                case null:
                    return false;

                default:
                    throw new InvalidQueryException($"Invalid {qnXsiNil} attribute value: `{value}`");
            }
        }

        internal static XName GetTypeAttributeValue(this XmlReader reader)
        {
            return GetTypeAttributeValue(reader, qnXsiType);
        }

        private static XName GetTypeAttributeValue(XmlReader reader, XName attributeName, bool isArrayType = false)
        {
            var typeValue = reader.GetAttribute(attributeName.LocalName, attributeName.NamespaceName);
            return ParseQualifiedValue(reader, typeValue, isArrayType);
        }

        internal static XName ParseQualifiedValue(this XmlReader reader, string value, bool isArrayType = false)
        {
            if (value == null)
                return null;

            var namespaceSeparatorIndex = value.IndexOf(':');
            var namespacePrefix = namespaceSeparatorIndex < 0 ? string.Empty : value.Substring(0, namespaceSeparatorIndex);
            var typeName = namespaceSeparatorIndex < 0 ? value : value.Substring(namespaceSeparatorIndex + 1);

            var typeNamespace = reader.LookupNamespace(namespacePrefix);
            if (typeNamespace == null)
                throw new InvalidQueryException($"Undefined namespace prefix `{namespacePrefix}` given in XML message for element `{reader.LocalName}` xsi:type.");

            if (isArrayType)
                typeName = typeName.Substring(0, typeName.LastIndexOf('['));

            var qualifiedName = XName.Get(typeName, typeNamespace);

            return qualifiedName != qnSoapEncArray ? qualifiedName : GetTypeAttributeValue(reader, qnSoapEncArrayType, true);
        }

        /// <summary>
        /// Reposition XML reader to matching end element of the current element.
        /// </summary>
        public static void ReadToEndElement(this XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            var currentDepth = reader.Depth;

            while (reader.Read() && currentDepth < reader.Depth)
            { }
        }

        /// <summary>
        /// Reposition XML reader to the next element if it's currently at nil element.
        /// </summary>
        public static void ConsumeNilElement(this XmlReader reader, bool isNil)
        {
            if (!isNil)
                return;

            if (reader.IsEmptyElement)
            {
                reader.Read();
                return;
            }

            var content = reader.ReadElementContentAsString();
            if (!string.IsNullOrEmpty(content))
                throw new InvalidQueryException($@"An element labeled with `xsi:nil=""true""` must be empty, but had `{content}` as content.");

            reader.ReadToEndElement();
        }

        /// <summary>
        /// Reposition reader at location where next Read() call will navigate to next node.
        /// </summary>
        public static void ConsumeUnusedElement(this XmlReader reader)
        {
            if (reader.IsEmptyElement) reader.Read();
            else reader.ReadToEndElement();
        }

        /// <summary>
        /// Check if XML reader is currently positioned at the specified element.
        /// </summary>
        public static bool IsCurrentElement(this XmlReader reader, int depth, XName name)
        {
            return reader.NodeType == XmlNodeType.Element && reader.Depth == depth && reader.LocalName == name.LocalName && reader.NamespaceURI == name.NamespaceName;
        }

        /// <summary>
        /// Move XML reader current position to next element which matches the given arguments.
        /// </summary>
        public static bool MoveToElement(this XmlReader reader, int depth, XName name = null)
        {
            while (true)
            {
                if (reader.Depth == depth && reader.NodeType == XmlNodeType.Element && (name == null || reader.IsCurrentElement(depth, name)))
                    return true;

                if (!reader.Read() || reader.Depth < depth)
                    return false;
            }
        }

        /// <summary>
        /// Check if current XML reader node is defined in one of the X-Road schema namespaces.
        /// </summary>
        public static bool IsHeaderNamespace(this XmlReader reader)
        {
            return headerNamespaces.Contains(reader.NamespaceURI);
        }

        /// <summary>
        /// Get current reader node name as XName.
        /// </summary>
        public static XName GetXName(this XmlReader reader)
        {
            return XName.Get(reader.LocalName, reader.NamespaceURI);
        }

        /// <summary>
        /// Deserialize current node as XRoadFault entity.
        /// </summary>
        public static IXRoadFault ReadXRoadFault(this XmlReader reader, int depth)
        {
            var fault = new XRoadFault();

            while (reader.Read() && reader.MoveToElement(depth))
            {
                if (reader.NodeType != XmlNodeType.Element)
                    continue;

                if (string.IsNullOrWhiteSpace(reader.NamespaceURI) && reader.LocalName == "faultCode")
                    fault.FaultCode = reader.ReadElementContentAsString();

                if (string.IsNullOrWhiteSpace(reader.NamespaceURI) && reader.LocalName == "faultString")
                    fault.FaultString = reader.ReadElementContentAsString();
            }

            return fault;
        }
    }
}