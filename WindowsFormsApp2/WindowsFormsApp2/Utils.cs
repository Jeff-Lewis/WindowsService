using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    class Utils
    {
        public static void LogToFile(string message, string filepath)
        {
            string datedMessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + WindowsApiWrapper.Username + " " + message;
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(datedMessage);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(datedMessage);
                }
            }
        }
    }
}
