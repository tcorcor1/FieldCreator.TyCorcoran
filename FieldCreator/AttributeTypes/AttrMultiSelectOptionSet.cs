using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrMultiSelectOptionSet : AttrBase, IAttribute
    {
        public AttrMultiSelectOptionSet(Attribute attribute) : base (attribute) {}

        public AttributeMetadata ReturnAttributeMetadata(Attribute attribute)
        {
            try
            {
                switch (attribute.OptionSetType)
                {
                    case "New Option Set":
                        var OptionSetAttr = new MultiSelectPicklistAttributeMetadata()
                        {
                            SchemaName = AttrSchemaName,
                            DisplayName = new Label(AttrFieldLabel, 1033),
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                            IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                            OptionSet = GenerateOptionSetMetadata(),
                            Description = (AttrDescription != null) ? new Label(AttrDescription, 1033) : null
                        };
                        return OptionSetAttr;

                    case "New Global Option Set":
                        var NewGlobalOptionSetAttr = new MultiSelectPicklistAttributeMetadata()
                        {
                            SchemaName = AttrSchemaName,
                            DisplayName = new Label(AttrFieldLabel, 1033),
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                            IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                            Description = (AttrDescription != null) ? new Label(AttrDescription, 1033) : null,
                            OptionSet = new OptionSetMetadata
                            {
                                IsGlobal = true,
                                Name = (string.IsNullOrWhiteSpace(attribute.GlobalOSSchemaName)) ? AttrSchemaName : attribute.GlobalOSSchemaName
                            }
                        };
                        return NewGlobalOptionSetAttr;

                    default:
                        var ExistGlobalOptionSetAttr = new MultiSelectPicklistAttributeMetadata()
                        {
                            SchemaName = AttrSchemaName,
                            DisplayName = new Label(AttrFieldLabel, 1033),
                            RequiredLevel = new AttributeRequiredLevelManagedProperty(AttrRequiredLevel),
                            IsAuditEnabled = new BooleanManagedProperty(AttrAuditEnabled),
                            Description = (AttrDescription != null) ? new Label(AttrDescription, 1033) : null,
                            OptionSet = new OptionSetMetadata
                            {
                                IsGlobal = true,
                                Name = attribute.ExistingGlobalOSSchemaName.ToLower()
                            }
                        };
                        return ExistGlobalOptionSetAttr;
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"{attribute.FieldSchemaName}: {ex.Message}");
            }
        }
    }
}
