using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace WebLogImport
{
    static class SQLConn
    {
        public static string GetConnect()
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
            string srLine;
            string connString = "";
            
            if(File.Exists(appPath + "\\connstring.dat"))
            {
                using (StreamReader srPath = new StreamReader(appPath + "\\connstring.dat"))
                {
                    while (srPath.Peek() >= 0)
                    {
                        srLine = srPath.ReadLine();
                        if (srLine.StartsWith("###"))
                        {
                            connString = srLine.Substring(3);
                        }
                    }
                    srPath.Close();
                }
            }

            return connString;            
        }

        public static bool WriteConnect(string ConnectionString)
        {
            string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

                using (StreamWriter swConn = new StreamWriter(appPath + "\\connstring.dat", false))
                {
                    swConn.WriteLine("[Enter your specific SQL connection string below, preceded by '###'.]");
                    swConn.WriteLine("###" + ConnectionString);
                    swConn.Flush();
                    swConn.Close();
                }

            return true;
        }

    }
}
