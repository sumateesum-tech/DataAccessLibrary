using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataAccessLibrary
{
    public static class LogHelper
    {
        public static void WriteLog(string message, Exception ex = null, string logPath = null)
        {
            try
            {
                if (string.IsNullOrEmpty(logPath))
                    logPath = AppDomain.CurrentDomain.BaseDirectory + "\\logs\\dataaccess.log";

                string directory = Path.GetDirectoryName(logPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + message;

                if (ex != null)
                    content += Environment.NewLine + "Exception: " + ex.ToString();

                content += Environment.NewLine + "----------------------------------------" + Environment.NewLine;

                File.AppendAllText(logPath, content);
            }
            catch
            {
                // Ignore log failure to avoid crashing the app
            }
        }
    }
}
