using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace FieldCreator.TyCorcoran
{
    public class FieldCreatorHelpers
    {
        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
        public static int ReturnProgressComplete(int index, int total)
        {
            var progress = (decimal) index/total;
            return Convert.ToInt16(progress * 100);
        }
        public static void PublishXml(BackgroundWorker worker, IOrganizationService service)
        {
            string xml = "<importexportxml><entities></entities></importexportxml>";
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNode parentNode = xmlDoc["importexportxml"];
            if (FieldCreatorPluginControl.ImportGlobalOptionSets.Count > 0)
            {
                XmlNode globalOptionSetNode = xmlDoc.CreateNode(XmlNodeType.Element, "optionsets", "");
                parentNode.AppendChild(globalOptionSetNode);

            }
            if (FieldCreatorPluginControl.ImportEntities.Count > 0)
            {
                IEnumerable<string> distinctEntityList = FieldCreatorPluginControl.ImportEntities.Distinct();
                XmlNode entitiesNode = xmlDoc["importexportxml"]["entities"];
                foreach (var entity in distinctEntityList)
                {
                    XmlNode entityNode = xmlDoc.CreateNode(XmlNodeType.Element, "entity", "");
                    entityNode.InnerText = entity;
                    entitiesNode.AppendChild(entityNode);
                }
            }
            worker.ReportProgress(100, "Publishing");
            var publishRequest = new PublishXmlRequest
            {
                ParameterXml = xmlDoc.OuterXml.ToString()
            };
            service.Execute(publishRequest);
        }
    }
}