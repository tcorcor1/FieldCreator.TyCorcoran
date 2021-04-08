using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace FieldCreator.TyCorcoran
{
    public class AttrBase
    {
        public AttrBase (Attribute attribute)
        {
            _attribute = attribute;
        }

        private Attribute _attribute;
        private const string _regexSanitizedSchemaName = "[^a-zA-Z0-9_]";
        public bool AttrAuditEnabled => _attribute.AuditEnabled.ToLower() == "true" ? true : false;
        public string AttrEntitySchemaName => Regex.Replace(_attribute.EntitySchemaName.ToLower(), _regexSanitizedSchemaName, string.Empty);
        public string AttrSolution => _attribute.SolutionUniqueName;
        public string AttrDescription => _attribute.Description;
        public string AttrSchemaName => Regex.Replace(_attribute.FieldSchemaName, _regexSanitizedSchemaName, string.Empty);
        public string AttrFieldLabel => _attribute.FieldLabel;
        public AttributeRequiredLevel AttrRequiredLevel
        {
            get
            {
                var reqlevel = new AttributeRequiredLevel();
                switch (_attribute.RequiredLevel)
                {
                    case "None":
                        reqlevel = AttributeRequiredLevel.None;
                        return reqlevel;
                    case "System Required":
                        reqlevel = AttributeRequiredLevel.ApplicationRequired;
                        return reqlevel;
                    case "Recommended":
                        reqlevel = AttributeRequiredLevel.Recommended;
                        return reqlevel;
                    default:
                        reqlevel = 0;
                        return reqlevel;
                }
            }
        }
        public OptionSetMetadata GenerateOptionSetMetadata()
        {
            var osMetaDataCollection = CreateOptionMetaDataCollection();
            return new OptionSetMetadata(osMetaDataCollection)
            {
                IsGlobal = false,
                OptionSetType = (_attribute.FieldType == "FieldCreator.TyCorcoran.AttrOptionSet") ? OptionSetType.Picklist : default
            };
        }
        public OptionMetadataCollection CreateOptionMetaDataCollection()
        {
            string optionSetValueString = _attribute.OptionSetValues;
            var optionSetMetadataCollection = new List<OptionMetadata>();
            if (!string.IsNullOrWhiteSpace(optionSetValueString))
            {
                var optionSetStringList = new List<string>(optionSetValueString.Split('|'));
                foreach (var option in optionSetStringList)
                {
                    var osMeta = new OptionMetadata(new Label(option, 1033), null);
                    optionSetMetadataCollection.Add(osMeta);
                }
            };
            var optionMetaCollection = new OptionMetadataCollection(optionSetMetadataCollection);
            return optionMetaCollection;
        }
        public static OptionMetadataCollection CreateOptionMetaDataCollection(Attribute attribute)
        {
            string optionSetValueString = attribute.OptionSetValues;
            var optionSetMetadataCollection = new List<OptionMetadata>();
            if (!string.IsNullOrWhiteSpace(optionSetValueString))
            {
                var optionSetStringList = new List<string>(optionSetValueString.Split('|'));
                foreach (var option in optionSetStringList)
                {
                    var osMeta = new OptionMetadata(new Label(option, 1033), null);
                    optionSetMetadataCollection.Add(osMeta);
                }
            };
            var optionMetaCollection = new OptionMetadataCollection(optionSetMetadataCollection);
            return optionMetaCollection;
        }
    }
}