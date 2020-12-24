using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KZ_Ingenico_EPI
{
    class LoggerPermanent
    {
        string File { get; set; }
        string Path { get; set; }
        StreamWriter sw;
        public LoggerPermanent(string fileName, string machineID)
        {
            this.File = fileName;
            this.Path = $@"{StringValue.WorkingDirectory}Log\{File}Trace-{machineID} {DateTime.Now:yyyy-MM-dd}.txt";
            sw = new StreamWriter(Path, true, Encoding.UTF8);
        }

        public void Write(string mess)
        {
            try
            {
                string dateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                sw.WriteLine("{0}: {1}", dateTime, mess);
            }
            catch { }
        }

        public void Close()
        {
            sw.Close();
        }
    }
}
