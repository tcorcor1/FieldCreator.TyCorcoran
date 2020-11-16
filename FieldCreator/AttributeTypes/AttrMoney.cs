using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrMoney : AttrBase, IAttribute
    {
        public AttrMoney(Attribute attribute) : base (attribute) {}

        public AttributeMetadata ReturnAttributeMetadata(Attribute attribute)
        {
            try
            {
                return new MoneyAttributeMetadata()
                {
                    SchemaName = AttrSchemaName,
                    DisplayName = new Label(AttrFieldLabel, 1033),
                    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                    IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                    MinValue = -1000000000,
                    MaxValue = 1000000000,
                    Precision = 2,
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
