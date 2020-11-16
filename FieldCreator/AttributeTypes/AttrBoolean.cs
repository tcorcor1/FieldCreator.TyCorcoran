using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrBoolean : AttrBase, IAttribute
    {
        public AttrBoolean(Attribute attribute) : base (attribute) {}

        public AttributeMetadata ReturnAttributeMetadata(Attribute attribute)
        {
            try
            {
                return new BooleanAttributeMetadata
                {
                    SchemaName = AttrSchemaName,
                    DisplayName = new Label(AttrFieldLabel, 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                    IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                    Description = (AttrDescription != null) ? new Label(AttrDescription, 1033) : null,
                    OptionSet = new BooleanOptionSetMetadata
                    (
                        new OptionMetadata(new Label("True", 1033), 1),
                        new OptionMetadata(new Label("False", 1033), 0)
                    ),
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"{attribute.FieldSchemaName}: {ex.Message}");
            }
        }
    }
}
