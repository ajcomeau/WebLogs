using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using WebLogImport;
using WebLogImport.logDefTableAdapters;
using Ionic.Zip;

namespace WebLogImport
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void cmdBrowse_Click(object sender, EventArgs e)
        {

            try
            {
                
                // If loading a single file, open the open file dialog and get the filename.
                // Otherwise, open the folder browser and get the path.

                if (rbFile.Checked)
                {
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.Filter = "log files (*.log)|*.log|Zip files (*.zip)|*.zip";

                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtFileName.Text = fileDialog.FileName;
                    }
                }
                else
                {
                    FolderBrowserDialog folderDialog = new FolderBrowserDialog();

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtFileName.Text = folderDialog.SelectedPath;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error ...");
            }
        }

        private void cmdLoad_Click(object sender, EventArgs e)
        {

            DataTable logTable = new DataTable();
            DataTable addTable = new DataTable();
            ZipFile zipLog;
            string logFile = "";
            
            
            // If the new file is being added to the records currently displayed, grab the datasource
            // for the grid for the merge.

            try
            {
                if (ckAddToList.Checked && dgFile.DataSource != null)
                    logTable = (DataTable)dgFile.DataSource;

                if (rbFile.Checked && txtFileName.Text.Length > 0)
                {                    
                    logFile = txtFileName.Text;

                    // If a ZIP file is selected, unzip it.
                    if (logFile.EndsWith(".zip"))
                    {
                        using (zipLog = new ZipFile(logFile))
                        {
                            zipLog.ExtractAll(Path.GetDirectoryName(logFile), ExtractExistingFileAction.OverwriteSilently);                            
                        }
                        File.Delete(logFile);
                        
                        // Assume that the log is named the same as the ZIP but with a different extension.
                        logFile = logFile.Substring(0, logFile.Length - 3) + "log";
                    }
                    
                    // Now process the file.
                    if (File.Exists(logFile))
                    {
                        // If the logTable has not been defined with columns yet, set it to the results of LoadFile.
                        // Otherwise, use addTable and merge it with logFile.
                        if (logTable.Columns.Count == 0)
                            logTable = IO.LoadFile(logFile, textDomain.Text);
                        else
                        {
                            addTable = IO.LoadFile(logFile, textDomain.Text);
                            logTable.Merge(addTable, true);
                        }
                    }
                }
                else
                {
                    if (txtFileName.Text.Length > 0 && Directory.Exists(txtFileName.Text))
                    {
                        // First unzip any compressed files in the directory and delete the ZIP files.
                        if (chkProcessZIP.Checked)
                        {
                            foreach (String zipFile in Directory.EnumerateFiles(txtFileName.Text, "*.zip"))
                            {
                                using (zipLog = new ZipFile(zipFile))
                                {
                                    zipLog.ExtractAll(txtFileName.Text, ExtractExistingFileAction.OverwriteSilently);
                                }
                                File.Delete(zipFile);
                            }
                        }

                        // Now iterate through the text log files and import them.
                        foreach (String logFileEntry in Directory.EnumerateFiles(txtFileName.Text, "*.log"))
                        {
                            // If the logTable has not been defined with columns yet, set it to the results of LoadFile.
                            // Otherwise, use addTable and merge it with logFile.
                            if (logTable.Columns.Count == 0)
                                logTable = IO.LoadFile(logFileEntry, textDomain.Text);
                            else
                            {
                                addTable = IO.LoadFile(logFileEntry, textDomain.Text);
                                logTable.Merge(addTable, true);
                            }
                        }
                    }
                }

                // Set the grid's datasource to logTable and update the status label.
                dgFile.DataSource = logTable;
                statusLabel1.Text = logTable.Rows.Count.ToString() + " records found.";
            }
            catch(Exception ex)
            {
                if (ex.Message.StartsWith("Error loading file"))
                {
                    MessageBox.Show("There was an error loading one of the log files. The file might be of the wrong format or corrupt." +
                        Environment.NewLine + ex.Message + Environment.NewLine + ex.InnerException.Message, "Error ...");
                }
                else
                    MessageBox.Show(ex.Message, "Error ...");
            }           

        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtLog = (DataTable)dgFile.DataSource;

                if (ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString.Length > 0)
                {
                    // Set status message
                    statusLabel1.Text = "Saving " + dtLog.Rows.Count + " log records ...";
                    statusLabel1.ForeColor = Color.Red;
                    statusStrip1.Refresh();

                    if (IO.SaveLogData(dtLog))
                    {
                        // Set the status message.
                        statusLabel1.Text = dtLog.Rows.Count + " log records saved.";
                        statusLabel1.ForeColor = Color.Black;
                        MessageBox.Show(dtLog.Rows.Count + " log records saved.", "Records saved.");
                        // Clear the datagrid.
                        dgFile.DataSource = null;
                    }
                }
                else
                {
                    MessageBox.Show("No connection information was found. Please update the confuguration information with a connection string for a SQL Server database.");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error ...");
            }

        }

        private void rbFile_CheckedChanged(object sender, EventArgs e)
        {
            txtFileName.Text = "";
        }

        private void rbDirectory_CheckedChanged(object sender, EventArgs e)
        {
            txtFileName.Text = "";
        }
    }
}
