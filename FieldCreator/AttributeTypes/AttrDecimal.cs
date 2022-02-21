using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrDecimal : AttrBase, IAttribute
    {
        public AttrDecimal (Attribute attribute) : base(attribute)
        {
        }

        public AttributeMetadata ReturnAttributeMetadata (Attribute attribute)
        {
            try
            {
                return new DecimalAttributeMetadata()
                {
                    SchemaName = AttrSchemaName,
                    DisplayName = new Label(AttrFieldLabel, CultureInfo.CurrentCulture.LCID),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                    IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                    MinValue = -1000000000,
                    MaxValue = 1000000000,
                    Precision = (string.IsNullOrWhiteSpace(attribute.Precision)) ? 2 : Convert.ToInt16(attribute.Precision),
                    Description = (AttrDescription != null) ? new Label(AttrDescription, CultureInfo.CurrentCulture.LCID) : null
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"{attribute.FieldSchemaName}: {ex.Message}");
            }
        }
    }
}