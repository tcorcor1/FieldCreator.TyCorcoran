using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrInteger : AttrBase, IAttribute
    {
        public AttrInteger (Attribute attribute) : base (attribute) {}

        public AttributeMetadata ReturnAttributeMetadata(Attribute attribute)
        {
            try
            {
                var maxValueWhole = (string.IsNullOrWhiteSpace(attribute.MaxValueWhole)) ? 2147483647 : Convert.ToInt32(attribute.MaxValueWhole);
                var minValueWhole = (string.IsNullOrWhiteSpace(attribute.MinValueWhole)) ? -2147483648 : Convert.ToInt32(attribute.MinValueWhole);
                return new IntegerAttributeMetadata()
                {
                    SchemaName = AttrSchemaName,
                    DisplayName = new Label(AttrFieldLabel, 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                    IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                    MaxValue = (maxValueWhole <= minValueWhole) ? 2147483647 : maxValueWhole,
                    MinValue = (minValueWhole >= maxValueWhole) ? -2147483648 : minValueWhole,
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
