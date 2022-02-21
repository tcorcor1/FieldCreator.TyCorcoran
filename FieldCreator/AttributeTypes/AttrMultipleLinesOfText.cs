using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrMultipleLinesOfText : AttrBase, IAttribute
    {
        public AttrMultipleLinesOfText (Attribute attribute) : base(attribute)
        {
        }

        public AttributeMetadata ReturnAttributeMetadata (Attribute attribute)
        {
            try
            {
                return new MemoAttributeMetadata()
                {
                    SchemaName = AttrSchemaName,
                    DisplayName = new Label(AttrFieldLabel, CultureInfo.CurrentCulture.LCID),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                    IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                    MaxLength = (string.IsNullOrWhiteSpace(attribute.MaxLengthMultiple)) ? 5000 : Convert.ToInt32(attribute.MaxLengthMultiple),
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