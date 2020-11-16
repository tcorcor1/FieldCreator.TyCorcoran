using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrSingleLineOfText : AttrBase, IAttribute
    {
        public AttrSingleLineOfText (Attribute attribute) : base (attribute) {}

        public AttributeMetadata ReturnAttributeMetadata(Attribute attribute)
        {
            try
            {
                return new StringAttributeMetadata()
                {
                    SchemaName = AttrSchemaName,
                    DisplayName = new Label(AttrFieldLabel, 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                    IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                    MaxLength = (string.IsNullOrWhiteSpace(attribute.MaxLengthSingle)) ? 500 : Convert.ToInt32(attribute.MaxLengthSingle),
                    Description = (AttrDescription != null) ? new Label(AttrDescription, 1033) : null
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"{attribute.FieldSchemaName}: {ex.Message}");
            }
        }
    }
}
