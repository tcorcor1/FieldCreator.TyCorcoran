using System;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    interface IAttribute
    {
        AttributeMetadata ReturnAttributeMetadata(Attribute attribute);
    }
}
