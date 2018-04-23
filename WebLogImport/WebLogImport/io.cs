using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
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
            int columnCount = 0;

            try
            {

                // Look for the first row with the field names to get Get column count and titles for the table.

                readLog = new StreamReader(FileName);
                while (!colDefined && readLog.Peek() >= 0)
                {
                    fileLine = readLog.ReadLine().Replace(" - - ", " "); // Remove blanks from Apachie Log.
                    if (columnCount == 0) { columnCount = fileLine.Split(' ').Length; };
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

                // If the columns have not been defined. Just add 
                // one for each field found.

                if (!colDefined)
                {
                    for (int x = 1; x <= columnCount + 1; x++)
                    {
                        dtLog.Columns.Add("Field" + x);
                    }
                }


                // Reopen the file and get the records.

                readLog = new StreamReader(FileName);

                while (readLog.Peek() >= 0)
                {
                    fileLine = readLog.ReadLine().Replace(" - - ", " "); ;
                    if (!fileLine.StartsWith("#"))
                    {
                        fileLine += " " + DomainName;
                        splitLine = fileLine.Split(' ');
                        dtLogRow = dtLog.NewRow();
                        // Reject any rows with too many columns.
                        if (splitLine.GetLength(0) == dtLog.Columns.Count)
                        {
                            dtLogRow.ItemArray = splitLine;
                            dtLog.Rows.Add(dtLogRow);
                        }
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

        public static bool SaveLogDataSQLite(DataTable LogDataTable)
        {
            // Not efficient for large numbers of records but leaving in as a demo.

            try
            {
                int result;
                string cs = ConfigurationManager.ConnectionStrings["SQLiteConnectionString"].ConnectionString;

                string sqlText = "INSERT INTO [weblogs] ([date], [time], [s-sitename], [s-computername], [s-ip], [cs-method], [cs-uri-stem], " +
                    "[cs-uri-query], [s-port], [cs-username], [c-ip], [cs-version], [cs(User-Agent)], [cs(Cookie)], [cs(Referrer)], [cs-host], " +
                    "[sc-status], [sc-substatus], [sc-win32-status], [sc-bytes], [cs-bytes], [time-taken], [site-name]) " +
                    "VALUES (@date, @time, @ssitename, @scomputername, @sip, @csmethod, @csuristem, " +
                    "@csuriquery, @sport, @csusername, @cip, @csversion, @csUserAgent, @csCookie, @csreferrer, @cshost, " +
                    "@scstatus, @scsubstatus, @scwin32status, @scbytes, @csbytes, @timetaken, @sitename);";
                SQLiteConnection liteConn = new SQLiteConnection(cs);
                SQLiteCommand liteCmd;
                liteConn.Open();                

                // Export row
                foreach (DataRow drLog in LogDataTable.Rows)
                {
                    if (drLog.ItemArray[13].ToString().Length > 8000)
                        drLog.ItemArray[13] = drLog.ItemArray[13].ToString().Substring(0, 8000);

                    liteCmd = new SQLiteCommand(sqlText, liteConn);
                    liteCmd.CommandType = CommandType.Text;
                    liteCmd.Parameters.AddWithValue("@date", drLog["date"].ToString());
                    liteCmd.Parameters.AddWithValue("@time", drLog["time"].ToString());
                    liteCmd.Parameters.AddWithValue("@ssitename", drLog["s-sitename"].ToString());
                    liteCmd.Parameters.AddWithValue("@scomputername", drLog["s-computername"].ToString());
                    liteCmd.Parameters.AddWithValue("@sip", drLog["s-ip"].ToString());
                    liteCmd.Parameters.AddWithValue("@csmethod", drLog["cs-method"].ToString());
                    liteCmd.Parameters.AddWithValue("@csuristem", drLog["cs-uri-stem"].ToString());
                    liteCmd.Parameters.AddWithValue("@csuriquery", drLog["cs-uri-query"]);
                    liteCmd.Parameters.AddWithValue("@sport", drLog["s-port"]);
                    liteCmd.Parameters.AddWithValue("@csusername", drLog["cs-username"]);
                    liteCmd.Parameters.AddWithValue("@cip", drLog["c-ip"]);
                    liteCmd.Parameters.AddWithValue("@csversion", drLog["cs-version"]);
                    liteCmd.Parameters.AddWithValue("@csUserAgent", drLog["cs(User-Agent)"]);
                    liteCmd.Parameters.AddWithValue("@csCookie", drLog["cs(Cookie)"]);
                    liteCmd.Parameters.AddWithValue("@csreferrer", drLog["cs(referer)"]);
                    liteCmd.Parameters.AddWithValue("@cshost", drLog["cs-host"]);
                    liteCmd.Parameters.AddWithValue("@scstatus", drLog["sc-status"]);
                    liteCmd.Parameters.AddWithValue("@scsubstatus", drLog["sc-substatus"]);
                    liteCmd.Parameters.AddWithValue("@scwin32status", drLog["sc-win32-status"]);
                    liteCmd.Parameters.AddWithValue("@scbytes", drLog["sc-bytes"]);
                    liteCmd.Parameters.AddWithValue("@csbytes", drLog["cs-bytes"]);
                    liteCmd.Parameters.AddWithValue("@timetaken", drLog["time-taken"]);
                    liteCmd.Parameters.AddWithValue("@sitename", drLog["SiteName"]);

                    result = liteCmd.ExecuteNonQuery();
                    
                }

                liteConn.Close();               
               
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving records to SQLite database.", ex);
            }
        }

        public static bool SaveLogDataSQLServer(DataTable LogDataTable)
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
                throw new Exception("Error saving records to SQL Server.", ex);
            }
        }
    }
}
