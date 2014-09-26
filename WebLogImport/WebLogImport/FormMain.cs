using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
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
                            logTable = WebLogImport.FormMain.LoadFile(logFile, textDomain.Text);
                        else
                        {
                            addTable = WebLogImport.FormMain.LoadFile(logFile, textDomain.Text);
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
                                logTable = WebLogImport.FormMain.LoadFile(logFileEntry, textDomain.Text);
                            else
                            {
                                addTable = WebLogImport.FormMain.LoadFile(logFileEntry, textDomain.Text);
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
                string cs = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
                DataTable dtLog = (DataTable)dgFile.DataSource;

                if (cs.Length > 0)
                {
                    if (dgFile.DataSource != null)
                    {
                        // Trim cookie field to 8000 characters
                        foreach (DataRow drLog in dtLog.Rows)
                        {
                            if (drLog.ItemArray[13].ToString().Length > 8000)
                                drLog.ItemArray[13] = drLog.ItemArray[13].ToString().Substring(0, 8000);
                        }

                        // Set status message
                        statusLabel1.Text = "Saving " + dtLog.Rows.Count + " log records ...";
                        statusLabel1.ForeColor = Color.Red;
                        statusStrip1.Refresh();

                        using (SqlConnection sqlConn = new SqlConnection(cs))
                        {
                            // Map the datatable columns to the SQL table columns and dump the records using SQLBulkCopy.
                            using (SqlBulkCopy sqlCopy = new SqlBulkCopy(cs))
                            {
                                sqlCopy.DestinationTableName = "Sample.dbo.weblogs";
                                sqlCopy.ColumnMappings.Add("date", "date");
                                sqlCopy.ColumnMappings.Add("time", "time");
                                sqlCopy.ColumnMappings.Add("s-sitename", "s-sitename");
                                sqlCopy.ColumnMappings.Add("s-computername", "s-computername");
                                sqlCopy.ColumnMappings.Add("s-ip", "s-ip");
                                sqlCopy.ColumnMappings.Add("cs-method", "cs-method");
                                sqlCopy.ColumnMappings.Add("cs-uri-stem", "cs-uri-stem");
                                sqlCopy.ColumnMappings.Add("cs-uri-query", "cs-uri-query");
                                sqlCopy.ColumnMappings.Add("s-port", "s-port");
                                sqlCopy.ColumnMappings.Add("cs-username", "cs-username");
                                sqlCopy.ColumnMappings.Add("c-ip", "c-ip");
                                sqlCopy.ColumnMappings.Add("cs-version", "cs-version");
                                sqlCopy.ColumnMappings.Add("cs(User-Agent)", "cs(user-agent)");
                                sqlCopy.ColumnMappings.Add("cs(Cookie)", "cs(cookie)");
                                sqlCopy.ColumnMappings.Add("cs(Referer)", "cs(referrer)");
                                sqlCopy.ColumnMappings.Add("cs-host", "cs-host");
                                sqlCopy.ColumnMappings.Add("sc-status", "sc-status");
                                sqlCopy.ColumnMappings.Add("sc-substatus", "sc-substatus");
                                sqlCopy.ColumnMappings.Add("sc-win32-status", "sc-win32-status");
                                sqlCopy.ColumnMappings.Add("sc-bytes", "sc-bytes");
                                sqlCopy.ColumnMappings.Add("cs-bytes", "cs-bytes");
                                sqlCopy.ColumnMappings.Add("time-taken", "time-taken");
                                sqlCopy.ColumnMappings.Add("SiteName", "site-name");
                                sqlCopy.WriteToServer(dtLog);
                            }
                        }

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
                    MessageBox.Show("No connection information found. Please enter the 'SQLConnectionString' connection string for a SQL Server database in the app.config file and try again.", "No connection found ...");
                }

                // PREVIOUS METHOD - Use the typed dataset (logDef.xsd) to define and save a recordset to the database.
                // Can be restored for saving to databases other than SQL Server.
                //if (dgFile.DataSource != null)
                //{
                //    weblogsTableAdapter tbaLog = new weblogsTableAdapter();
                //    // DataTable dtLog = (DataTable)dgFile.DataSource;
                //    logDef.weblogsDataTable dtExport = new logDef.weblogsDataTable();

                //    // For each row in the datagrid, add a new row to the weblogsDataTable and fill in the values.
                //    foreach (DataRow drLog in dtLog.Rows)
                //    {
                //        logDef.weblogsRow rwExport = (logDef.weblogsRow)dtExport.NewRow();

                //        rwExport.date = DateTime.Parse(drLog.ItemArray[0].ToString());
                //        rwExport.time = drLog.ItemArray[1].ToString();
                //        rwExport._s_sitename = drLog.ItemArray[2].ToString();
                //        rwExport._s_computername = drLog.ItemArray[3].ToString();
                //        rwExport._s_ip = drLog.ItemArray[4].ToString();
                //        rwExport._cs_method = drLog.ItemArray[5].ToString();
                //        rwExport._cs_uri_stem = drLog.ItemArray[6].ToString();
                //        rwExport._cs_uri_query = drLog.ItemArray[7].ToString();
                //        rwExport._s_port = drLog.ItemArray[8].ToString();
                //        rwExport._cs_username = drLog.ItemArray[9].ToString();
                //        rwExport._c_ip = drLog.ItemArray[10].ToString();
                //        rwExport._cs_version = drLog.ItemArray[11].ToString();
                //        rwExport._cs_user_agent_ = drLog.ItemArray[12].ToString();
                //        // Max length for varchar field - 8000.
                //        if (drLog.ItemArray[12].ToString().Length > 8000)
                //            rwExport._cs_cookie_ = drLog.ItemArray[13].ToString().Substring(0, 8000);
                //        else
                //            rwExport._cs_cookie_ = drLog.ItemArray[13].ToString();
                //        rwExport._cs_referrer_ = drLog.ItemArray[14].ToString();
                //        rwExport._cs_host = drLog.ItemArray[15].ToString();
                //        rwExport._sc_status = drLog.ItemArray[16].ToString();
                //        rwExport._sc_substatus = drLog.ItemArray[17].ToString();
                //        rwExport._sc_win32_status = drLog.ItemArray[18].ToString();
                //        rwExport._sc_bytes = int.Parse(drLog.ItemArray[19].ToString());
                //        rwExport._cs_bytes = int.Parse(drLog.ItemArray[20].ToString());
                //        rwExport._time_taken = int.Parse(drLog.ItemArray[21].ToString());
                //        rwExport._site_name = drLog.ItemArray[22].ToString();

                //        dtExport.AddweblogsRow(rwExport);
                //    }

                //    statusLabel1.Text = "Saving " + dtExport.Rows.Count + " log records ...";
                //    statusLabel1.ForeColor = Color.Red;
                //    statusStrip1.Refresh();

                //    // Update SQL Server with the data table.
                //    tbaLog.Update(dtExport);

                //    statusLabel1.Text = dtExport.Rows.Count + " log records saved.";
                //    statusLabel1.ForeColor = Color.Black;
                //    MessageBox.Show(dtExport.Rows.Count + " log records saved.", "Records saved.");

                //    // Clear the datagrid.
                //    dgFile.DataSource = null;
                //}
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
