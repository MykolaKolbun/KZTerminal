using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KZ_Ingenico_EPI
{
    class Logger
    {
        /// <summary>
        /// Полный путь к лог файлу.
        /// </summary>
        //string filePath = "C:\\Log\\FiscalTrace"+DateTime.Now.Date.ToString()+".txt";
        string filePath = string.Format($@"C:\Log\IngenicoTrace - {DateTime.Now:yyyy-MM-dd}.txt");

        string File { get; set; }
        string Path { get; set; }

        public Logger(string fileName, string machineID)
        {
            this.File = fileName;
            this.Path = $@"C:\Log\{File}Trace-{machineID} {DateTime.Now:yyyy-MM-dd}.txt";
        }

        public void Write(string mess)
        {
            string dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            StreamWriter sw = new StreamWriter(Path, true, Encoding.UTF8);
            sw.WriteLine("{0}: {1}", dateTime, mess);
            sw.Close();
        }
    }
}
