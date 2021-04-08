using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Threading;

namespace FieldCreator.TyCorcoran
{
    public class Attribute
    {
        public static Dictionary<string, string> AttrTypes = new Dictionary<string, string>
        {
            {"Single Line of Text", "FieldCreator.TyCorcoran.AttrSingleLineOfText"},
            {"Multiple Lines of Text", "FieldCreator.TyCorcoran.AttrMultipleLinesOfText"},
            {"Whole Number", "FieldCreator.TyCorcoran.AttrInteger"},
            {"Date and Time", "FieldCreator.TyCorcoran.AttrDateTime"},
            {"Option Set", "FieldCreator.TyCorcoran.AttrOptionSet"},
            {"Multi-Select Option Set", "FieldCreator.TyCorcoran.AttrMultiSelectOptionSet"},
            {"Decimal Number", "FieldCreator.TyCorcoran.AttrDecimal"},
            {"Two Options", "FieldCreator.TyCorcoran.AttrBoolean"},
            {"Currency", "FieldCreator.TyCorcoran.AttrMoney"},
            {"Lookup", "FieldCreator.TyCorcoran.AttrLookup"}
        };
        
        [Name("Field Type")]
        public string FieldType { get; set; }
        
        [Name("Entity Schema Name")]
        public string EntitySchemaName { get; set; }
        
        [Name("Field Label")]
        public string FieldLabel { get; set; }
        
        [Name("Field Schema Name")]
        public string FieldSchemaName { get; set; }
        
        [Name("Required Level")]
        public string RequiredLevel { get; set; }
        
        [Name("Solution Unique Name")]
        public string SolutionUniqueName { get; set; }
        
        [Name("Description")]
        public string Description { get; set; }
        
        [Name("Audit Enabled")]
        public string AuditEnabled { get; set; }
        
        [Name("Option Set Type")]
        public string OptionSetType { get; set; }
        
        [Name("Option Set Values")]
        public string OptionSetValues { get; set; }
        
        [Name("New Global Option Set Display Name")]
        public string GlobalOSDisplayName { get; set; }
        
        [Name("New Global Option Set Schema Name")]
        public string GlobalOSSchemaName { get; set; }
        
        [Name("Existing Global Option Set Schema Name")]
        public string ExistingGlobalOSSchemaName { get; set; }
        
        [Name("Referenced Entity")]
        public string ReferencedEntity { get; set; }
        
        [Name("One > N Relationship Schema Name")]
        public string OnetoNRelationshipSchemaName { get; set; }
        
        [Name("Max Length (Single Line of Text)")]
        public string MaxLengthSingle { get; set; }
        
        [Name("Max Length (Multiple Lines of Text)")]
        public string MaxLengthMultiple { get; set; }
        
        [Name("Max Value (Whole Number)")]
        public string MaxValueWhole { get; set; }
        
        [Name("Min Value (Whole Number)")]
        public string MinValueWhole { get; set; }
        
        [Name("Precision")]
        public string Precision { get; set; }
        
        public static List<Attribute> ReturnAttributeList(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.CurrentCulture))
            {
                var csvAttributeList = csv.GetRecords<Attribute>().ToList();
                if (csvAttributeList.Count == 0)
                    MessageBox.Show("Csv was empty or ill-formatted");
                return csvAttributeList;
            }
        }

        public static void ProcessGlobalOptionSetAttributeList(BackgroundWorker worker, List<Attribute> attributeList, IOrganizationService service)
        {
            worker.ReportProgress(0, "Reviewing/Processing Global Option Sets");
            var globalOptionSetList = new List<Attribute>();
            foreach (var attribute in attributeList)
            {
                if (attribute.OptionSetType == "New Global Option Set")
                {
                    globalOptionSetList.Add(attribute);
                }
            }
            if (globalOptionSetList.Count != 0)
            {
                foreach (var attribute in globalOptionSetList)
                {
                    string optionSetSchemaName = (string.IsNullOrWhiteSpace(attribute.GlobalOSSchemaName)) ? attribute.FieldSchemaName : attribute.GlobalOSSchemaName;
                    string optionSetDisplayName = (string.IsNullOrWhiteSpace(attribute.GlobalOSDisplayName)) ? attribute.FieldLabel : attribute.GlobalOSDisplayName;
                    string regexSantizedName = "[^a-zA-Z0-9_]";
                    OptionMetadataCollection globalOSCollection = AttrBase.CreateOptionMetaDataCollection(attribute);
                    var createOptionSetMeta = new OptionSetMetadata(globalOSCollection)
                    {
                        Name = Regex.Replace(optionSetSchemaName, regexSantizedName, string.Empty),
                        DisplayName = new Label(optionSetDisplayName, 1033),
                        IsGlobal = true,
                        OptionSetType = Microsoft.Xrm.Sdk.Metadata.OptionSetType.Picklist
                    };
                    var createOptionsetReq = new CreateOptionSetRequest
                    {
                        OptionSet = createOptionSetMeta,
                        SolutionUniqueName = attribute.SolutionUniqueName
                    };
                    try
                    {
                        service.Execute(createOptionsetReq);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                FieldCreatorPluginControl.ImportGlobalOptionSets = globalOptionSetList;
            }
        }

        public static void ProcessAttributeList(BackgroundWorker worker, List<Attribute> attributeList, IOrganizationService service)
        {
            var importLogs = new List<string>();
            var importEntities = new List<string>();
            int successfulImports = 0;
            int failedImports = 0;
            foreach (var attr in attributeList)
            {
                int progressComplete = FieldCreatorHelpers.ReturnProgressComplete(attributeList.IndexOf(attr), attributeList.Count());
                worker.ReportProgress(progressComplete, attr.FieldSchemaName);

                string fullyQualFieldTypeName = AttrTypes[attr.FieldType];
                Type attributeType = Type.GetType(fullyQualFieldTypeName);
                dynamic attribute = Activator.CreateInstance(attributeType, attr);
                var attributeInstance = (IAttribute)attribute;
                AttributeMetadata attributeMetadata = attributeInstance.ReturnAttributeMetadata(attr);
                if (attr.FieldType == "Lookup")
                {
                    CreateOneToManyRequest createOneToManyRelationshipRequest = new CreateOneToManyRequest
                    {
                        OneToManyRelationship = new OneToManyRelationshipMetadata
                        {
                            ReferencedEntity = attr.ReferencedEntity,
                            ReferencingEntity = attribute.AttrEntitySchemaName,
                            SchemaName = attr.OnetoNRelationshipSchemaName,
                            AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                            {
                                Behavior = AssociatedMenuBehavior.UseLabel,
                                Group = AssociatedMenuGroup.Details,
                                Label = new Label(attribute.AttrEntitySchemaName, 1033),
                                Order = 10000
                            },
                            CascadeConfiguration = new CascadeConfiguration
                            {
                                Assign = CascadeType.NoCascade,
                                Delete = CascadeType.RemoveLink,
                                Merge = CascadeType.NoCascade,
                                Reparent = CascadeType.NoCascade,
                                Share = CascadeType.NoCascade,
                                Unshare = CascadeType.NoCascade
                            }
                        },
                        Lookup = (LookupAttributeMetadata)attributeMetadata,
                        SolutionUniqueName = attr.SolutionUniqueName
                    };
                    try
                    {
                        service.Execute(createOneToManyRelationshipRequest);
                        importLogs.Add($"Success | {attr.FieldSchemaName}");
                        importEntities.Add(attr.EntitySchemaName);
                        successfulImports++;
                    }
                    catch (Exception exception)
                    {
                        importLogs.Add($"Fail | {attr.FieldSchemaName} - {exception.Message}");
                        failedImports++;
                    }
                }
                else
                {
                    CreateAttributeRequest createAttrRequest = new CreateAttributeRequest
                    {
                        SolutionUniqueName = attribute.AttrSolution,
                        EntityName = attribute.AttrEntitySchemaName,
                        Attribute = attributeMetadata
                    };
                    try
                    {
                        service.Execute(createAttrRequest);
                        importLogs.Add($"Success | {attr.FieldSchemaName}");
                        importEntities.Add(attr.EntitySchemaName);
                        successfulImports++;
                    }
                    catch (Exception exception)
                    {
                        importLogs.Add($"Fail | {attr.FieldSchemaName} - {exception.Message}");
                        failedImports++;
                    }
                }
            }
            FieldCreatorPluginControl.ImportEntities = importEntities;
            FieldCreatorPluginControl.ImportLogs = importLogs;
        }
    }
}
