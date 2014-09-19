using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
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
                    fileDialog.Filter = "log files (*.log)|*.log|All files (*.*)|*.*";

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
            
            
            // If the new file is being added to the records currently displayed, grab the datasource
            // for the grid for the merge.

            try
            {
                if (ckAddToList.Checked && dgFile.DataSource != null)
                    logTable = (DataTable)dgFile.DataSource;

                if (rbFile.Checked)
                {
                    if (txtFileName.TextLength > 0 && File.Exists(txtFileName.Text))
                    {
                        // If the logTable has not been defined with columns yet, set it to the results of LoadFile.
                        // Otherwise, use addTable and merge it with logFile.
                        if (logTable.Columns.Count == 0)
                            logTable = WebLogImport.FormMain.LoadFile(txtFileName.Text, textDomain.Text);
                        else
                        {
                            addTable = WebLogImport.FormMain.LoadFile(txtFileName.Text, textDomain.Text);
                            logTable.Merge(addTable, true);
                        }
                    }
                }
                else
                {
                    if (Directory.Exists(txtFileName.Text))
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
                        foreach (String logFile in Directory.EnumerateFiles(txtFileName.Text, "*.log"))
                        {
                            // If the logTable has not been defined with columns yet, set it to the results of LoadFile.
                            // Otherwise, use addTable and merge it with logFile.
                            if (logTable.Columns.Count == 0)
                                logTable = WebLogImport.FormMain.LoadFile(logFile, textDomain.Text);
                            else
                            {
                                addTable = WebLogImport.FormMain.LoadFile(logFile, textDomain.Text);
                                logTable.Merge(addTable, true);
                            }
                        }
                    }
                }

                // Set the grid's datasource to logTable and upate the status label.

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

        public static DataTable LoadFile(string FileName, string DomainName)
        {
            DataTable dtLog = new DataTable();
            DataRow dtLogRow;
            StreamReader readLog;
            string fileLine;
            List<string> titleList;
            string[] splitLine;
            bool colDefined = false;

            try
            {

                // Look for the first row with the field names to get Get column count and titles for the table.

                readLog = new StreamReader(FileName);
                while (!colDefined)
                {
                    fileLine = readLog.ReadLine();
                    if (fileLine.StartsWith("#Fields: "))
                    {
                        fileLine = fileLine.Remove(0, 9);
                        titleList = fileLine.Split(' ').ToList();
                        foreach (string title in titleList)
                        {
                            dtLog.Columns.Add(title);
                        }
                        dtLog.Columns.Add("SiteName");
                        colDefined = true;
                    }
                }

                readLog.Close();

                // Reopen the file and get the records.

                readLog = new StreamReader(FileName);

                while (readLog.Peek() >= 0)
                {
                    fileLine = readLog.ReadLine();
                    if (!fileLine.StartsWith("#"))
                    {
                        fileLine += " " + DomainName;
                        splitLine = fileLine.Split(' ');
                        dtLogRow = dtLog.NewRow();
                        dtLogRow.ItemArray = splitLine;
                        dtLog.Rows.Add(dtLogRow);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error loading file " + FileName + ".", ex);
            }

            // Return the table.
            return dtLog;

        }

        private void cmdSave_Click(object sender, EventArgs e)
        {

            try
            {
                if (dgFile.DataSource != null)
                {
                    // Use the strongly typed dataset (logDef.xsd) to define and save a recordset to the database.
                    weblogsTableAdapter tbaLog = new weblogsTableAdapter();
                    DataTable dtLog = (DataTable)dgFile.DataSource;
                    logDef.weblogsDataTable dtExport = new logDef.weblogsDataTable();

                    // For each row in the datagrid, add a new row to the weblogsDataTable and fill in the values.
                    foreach (DataRow drLog in dtLog.Rows)
                    {
                        logDef.weblogsRow rwExport = (logDef.weblogsRow)dtExport.NewRow();

                        rwExport.date = DateTime.Parse(drLog.ItemArray[0].ToString());
                        rwExport.time = drLog.ItemArray[1].ToString();
                        rwExport._s_sitename = drLog.ItemArray[2].ToString();
                        rwExport._s_computername = drLog.ItemArray[3].ToString();
                        rwExport._s_ip = drLog.ItemArray[4].ToString();
                        rwExport._cs_method = drLog.ItemArray[5].ToString();
                        rwExport._cs_uri_stem = drLog.ItemArray[6].ToString();
                        rwExport._cs_uri_query = drLog.ItemArray[7].ToString();
                        rwExport._s_port = drLog.ItemArray[8].ToString();
                        rwExport._cs_username = drLog.ItemArray[9].ToString();
                        rwExport._c_ip = drLog.ItemArray[10].ToString();
                        rwExport._cs_version = drLog.ItemArray[11].ToString();
                        rwExport._cs_user_agent_ = drLog.ItemArray[12].ToString();
                        // Max length for varchar field - 8000.
                        if (drLog.ItemArray[12].ToString().Length > 8000)
                            rwExport._cs_cookie_ = drLog.ItemArray[13].ToString().Substring(0, 8000);
                        else
                            rwExport._cs_cookie_ = drLog.ItemArray[13].ToString();
                        rwExport._cs_referrer_ = drLog.ItemArray[14].ToString();
                        rwExport._cs_host = drLog.ItemArray[15].ToString();
                        rwExport._sc_status = drLog.ItemArray[16].ToString();
                        rwExport._sc_substatus = drLog.ItemArray[17].ToString();
                        rwExport._sc_win32_status = drLog.ItemArray[18].ToString();
                        rwExport._sc_bytes = int.Parse(drLog.ItemArray[19].ToString());
                        rwExport._cs_bytes = int.Parse(drLog.ItemArray[20].ToString());
                        rwExport._time_taken = int.Parse(drLog.ItemArray[21].ToString());
                        rwExport._site_name = drLog.ItemArray[22].ToString();

                        dtExport.AddweblogsRow(rwExport);
                    }

                    statusLabel1.Text = "Saving " + dtExport.Rows.Count + " log records ...";
                    statusLabel1.ForeColor = Color.Red;
                    statusStrip1.Refresh();

                    // Update SQL Server with the data table.
                    tbaLog.Update(dtExport);

                    statusLabel1.Text = dtExport.Rows.Count + " log records saved.";
                    statusLabel1.ForeColor = Color.Black;
                    MessageBox.Show(dtExport.Rows.Count + " log records saved.", "Records saved.");

                    // Clear the datagrid.
                    dgFile.DataSource = null;
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
