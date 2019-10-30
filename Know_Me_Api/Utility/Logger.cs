using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;

namespace Know_Me_Api.Utility
{
    public class Logger
    {
        private readonly IConfiguration _config;

        public Logger(IConfiguration config)
        {
            _config = config;
        }

        public static void LogError(Exception ex)
        {
            CreateLogFile();
            var path = GetFilePath();

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine();
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Error Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
            return;
        }

        public static void Log(string msg)
        {
            CreateLogFile();
            var path = GetFilePath();

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine();
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();
                writer.WriteLine("Message : " + msg);
            }
            return;
        }

        private static void CreateLogFile()
        {
            string path = GetFilePath();
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine();
                writer.WriteLine("\n\n***********************Log file for the date" + DateTime.Now.Date.ToString() + "************************\n\n");
                writer.WriteLine();
            }
            return;
        }

        private static string GetFilePath()
        {
            var filePath = @"D:\logs\";
            var fileName = DateTime.Now.Date.ToString() + ".txt";
            return filePath + fileName;
        }
    }
}
