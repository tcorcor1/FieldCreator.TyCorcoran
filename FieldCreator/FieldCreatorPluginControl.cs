using System;
using System.Collections.Generic;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace FieldCreator.TyCorcoran
{
    public partial class FieldCreatorPluginControl : PluginControlBase
    {
        private Settings mySettings;
        public static List<string> ImportLogs = new List<string> ();
        public static List<string> ImportEntities = new List<string> ();
        public static List<Attribute> ImportGlobalOptionSets = new List<Attribute>();

        public FieldCreatorPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

        private void BuildAttributes()
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Preparing...",

                Work = (worker, args) =>
                {
                    List<Attribute> attributeList = Attribute.ReturnAttributeList(txt_path.Text);
                    Attribute.ProcessGlobalOptionSetAttributeList(worker, attributeList, Service);
                    Attribute.ProcessAttributeList(worker, attributeList, Service);
                    FieldCreatorHelpers.PublishXml(worker, Service);
                },

                ProgressChanged = (args) =>
                {
                    SetWorkingMessage($"Attributes Processed {args.ProgressPercentage.ToString()}%\n" + args.UserState);
                },

                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        lst_csvlines.DataSource = ImportLogs;
                        btn_browse.Enabled = false;
                        btn_submit.Enabled = false;
                        btn_export.Enabled = true;
                        btn_refresh.Enabled = true;
                    }
                }
            });
        }

        private void btn_downloadtemplate_Click(object sender, EventArgs e)
        {
            var saveTemplate = new SaveFileDialog
            {
                Filter = @"Excel File|*.xlsx",
                FileName = "FieldCreator_Template.xlsx",
                OverwritePrompt = true
            };
            if (saveTemplate.ShowDialog(this) == DialogResult.OK)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var template = "FieldCreator.TyCorcoran.FieldCreator.Templates.FieldCreator_Template.xlsx";
                using (Stream stream = assembly.GetManifestResourceStream(template))
                {
                    using (FileStream file = new FileStream(saveTemplate.FileName, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(file);
                    }
                }
                MessageBox.Show("Template Downloaded");
            }
        }
        
        private void btn_browse_Click(object sender, EventArgs e)
        {
            var selectedFile = new OpenFileDialog
            {
                InitialDirectory = @"c:\",
                Title = "Browse Text Files",
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "csv files (*.csv)|*.csv",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (selectedFile.ShowDialog() == DialogResult.OK)
            {
                txt_path.Text = selectedFile.FileName;
            }
        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_path.Text))
            {
                MessageBox.Show("You must select a csv before proceeding");
            }
            else
            {
                var uploadedFile = new FileInfo(txt_path.Text);
                bool isFileLocked = FieldCreatorHelpers.IsFileLocked(uploadedFile);
                if (!isFileLocked)
                {
                    ExecuteMethod(BuildAttributes);
                }
                else
                {
                    MessageBox.Show("File is currently locked. Please make sure the csv is not open");
                }
            }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            string uploadPath = txt_path.Text;
            int index = uploadPath.LastIndexOf(@"\") + 1;
            string exportPath = uploadPath.Substring(0, index);
            try
            {
                using (StreamWriter sw = new StreamWriter($"{exportPath}FieldCreator_results.csv"))
                {
                    var importLogs = (List<string>)lst_csvlines.DataSource;
                    foreach (var ImportLog in importLogs)
                    {
                        sw.WriteLine(ImportLog);
                    }
                }
                MessageBox.Show($"Results downloaded to {exportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            };
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            txt_path.Clear();
            lst_csvlines.DataSource = null;
            btn_submit.Enabled = true;
            btn_browse.Enabled = true;
            btn_export.Enabled = false;
            btn_refresh.Enabled = false;
        }

        private void lklbl_documentation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://tldr-dynamics.com/blog/field-creator/");
            e.Link.Visited = true;
        }
    }
}