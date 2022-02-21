using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrLookup : AttrBase, IAttribute
    {
        public AttrLookup (Attribute attribute) : base(attribute)
        {
        }

        public AttributeMetadata ReturnAttributeMetadata (Attribute attribute)
        {
            try
            {
                return new LookupAttributeMetadata
                {
                    SchemaName = AttrSchemaName,
                    DisplayName = new Label(AttrFieldLabel, CultureInfo.CurrentCulture.LCID),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                    IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
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