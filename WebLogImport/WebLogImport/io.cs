using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace WebLogImport
{
    static class IO
    {
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

                // Return the table.
                return dtLog;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading file " + FileName + ".", ex);
            }
        }

        public static bool SaveLogData(DataTable LogDataTable)
        {
            try
            {
                string cs = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;

                // Trim cookie field to 8000 characters
                foreach (DataRow drLog in LogDataTable.Rows)
                {
                    if (drLog.ItemArray[13].ToString().Length > 8000)
                        drLog.ItemArray[13] = drLog.ItemArray[13].ToString().Substring(0, 8000);
                }

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
                        sqlCopy.WriteToServer(LogDataTable);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving records.", ex);
            }
        }
    }
}
