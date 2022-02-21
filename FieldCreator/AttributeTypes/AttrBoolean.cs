using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrBoolean : AttrBase, IAttribute
    {
        public AttrBoolean (Attribute attribute) : base(attribute)
        {
        }

        public AttributeMetadata ReturnAttributeMetadata (Attribute attribute)
        {
            try
            {
                return new BooleanAttributeMetadata
                {
                    SchemaName = AttrSchemaName,
                    DisplayName = new Label(AttrFieldLabel, CultureInfo.CurrentCulture.LCID),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                    IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                    Description = (AttrDescription != null) ? new Label(AttrDescription, CultureInfo.CurrentCulture.LCID) : null,
                    OptionSet = new BooleanOptionSetMetadata
                    (
                        new OptionMetadata(new Label("True", CultureInfo.CurrentCulture.LCID), 1),
                        new OptionMetadata(new Label("False", CultureInfo.CurrentCulture.LCID), 0)
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