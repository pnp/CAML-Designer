using System;
using System.IO;

namespace CamlDesigner2013.Helpers
{
    /// <summary>
    /// Logger class
    /// </summary>
    public static class Logger
    {
        public static void WriteToLogFile(string logMessage, Exception ex)
        {
            string strLogMessage = string.Empty;
            string strLogFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\BIWUG\CamlDesigner" + @"\logfile.log";
            StreamWriter streamWriterLog;
            if (ex == null)
            {
                strLogMessage = string.Format("{0}: {1}", DateTime.Now, logMessage);
            }
            else
            {
                strLogMessage = string.Format("{0}: {1}", DateTime.Now, logMessage + ex.Message + " - " + ex.StackTrace.ToString());
            }

            // check if file exists or not 
            if (File.Exists(strLogFile))
            {
                using (FileStream fs = new FileStream(strLogFile,FileMode.Append))
                {
                    streamWriterLog = new StreamWriter(fs);
                    streamWriterLog.WriteLine(strLogMessage);
                    streamWriterLog.WriteLine();
                    streamWriterLog.Close();
                }
            }
            else
            {
                using (FileStream fs = new FileStream(strLogFile, FileMode.OpenOrCreate))
                {
                   
                        streamWriterLog = new StreamWriter(fs);
                        streamWriterLog.WriteLine(strLogMessage);
                        streamWriterLog.WriteLine();
                        streamWriterLog.Close();
                }
            }
        }
    }
}
