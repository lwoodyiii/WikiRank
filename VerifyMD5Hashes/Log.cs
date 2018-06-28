using System;
using System.IO;

namespace VerifyMD5Hashes
{
    class Log
    {
        public string logPath;
        private StreamWriter logFileWriter;

        public Log(string lPath)
        {
            string date = DateTime.Now.ToShortDateString();
            date = date.Replace("/", "-");
            logPath = lPath + @"\MD5VerifyLog" + date + ".txt";
            logFileWriter = new StreamWriter(logPath);
            logFileWriter.AutoFlush = true;
        }

        public void Write(string logItem)
        {
            Console.WriteLine(logItem);
            logFileWriter.WriteLine(logItem);
        }

        
    }
}
