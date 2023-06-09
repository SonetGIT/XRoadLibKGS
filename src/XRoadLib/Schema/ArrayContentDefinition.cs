﻿using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using XRoadLib.Attributes;
using XRoadLib.Extensions;

namespace XRoadLib.Schema
{
    public class ArrayContentDefiniton : ContentDefinition
    {
        public override bool IsOptional
        {
            get => base.IsOptional || (MergeContent && Item.MinOccurs == 0u);
            set => base.IsOptional = value;
        }

        public ArrayItemDefinition Item { get; set; }

        public override XName SerializedName => MergeContent ? Item.Content.Name : Name;

        public ArrayContentDefiniton(ParticleDefinition particle, ICustomAttributeProvider customAttributeProvider, Type runtimeType, string runtimeName, string targetNamespace, bool defaultQualifiedElement)
            : base(particle)
        {
            if (customAttributeProvider.GetXmlElementAttribute() != null)
                throw new SchemaDefinitionException($"Array content property `{particle} ({runtimeName})` should not use XmlElement attribute in definition.");

            var arrayAttribute = customAttributeProvider.GetXmlArrayAttribute();
            var xroadArrayAttribute = arrayAttribute as XRoadXmlArrayAttribute;

            var arrayItemAttribute = customAttributeProvider.GetXmlArrayItemAttribute();
            var xroadArrayItemAttribute = arrayItemAttribute as XRoadXmlArrayItemAttribute;

            if (runtimeType.GetArrayRank() > 1)
                throw new SchemaDefinitionException($"Property `{particle}` declares multi-dimensional array, which is not supported.");

            Name = XName.Get(
                (arrayAttribute?.ElementName).GetStringOrDefault(runtimeName),
                GetQualifiedNamespace(
                    arrayAttribute?.Namespace ?? "",
                    arrayAttribute?.Form,
                    targetNamespace,
                    defaultQualifiedElement
                )
            );

            IsNullable = (arrayAttribute?.IsNullable).GetValueOrDefault();
            Order = (arrayAttribute?.Order).GetValueOrDefault(-1);
            UseXop = typeof(Stream).GetTypeInfo().IsAssignableFrom(runtimeType) && (xroadArrayItemAttribute?.UseXop).GetValueOrDefault(true);
            TypeName = (arrayItemAttribute?.DataType).MapNotEmpty(x => XName.Get(x, NamespaceConstants.XSD));
            IsOptional = xroadArrayAttribute?.IsOptional == true;
            State = DefinitionState.Default;
            Documentation = new DocumentationDefinition(customAttributeProvider);
            MergeContent = customAttributeProvider.HasMergeAttribute();
            RuntimeType = runtimeType;

            Item = new ArrayItemDefinition(
                Particle,
                arrayItemAttribute,
                runtimeType.GetElementType(),
                MergeContent ? runtimeName : "item",
                targetNamespace,
                defaultQualifiedElement
            );
        }
    }
}