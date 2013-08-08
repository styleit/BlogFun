using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlogFun
{
    public class BlogFunUtlity
    {
        public static string GetURLContents(string url)
        {
            string content = string.Empty;

            var webReq = (HttpWebRequest)WebRequest.Create(url);

            using (WebResponse response = webReq.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream);
                    content = reader.ReadToEnd();
                }
            }
            return content;
        }

        public static void ExecuteCmd(string command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.StandardInput.WriteLine(command);

            p.StandardInput.WriteLine("exit");
            p.WaitForExit();
            p.Close();
        }

        public static string filterTitle(string title)
        {
            char[] filter = new char[] {'*', '?', '/', '\\', '|', ':', '\"', '>', '<', '\'', '\r', '\n' };
            foreach (char item in filter)
            {
                title = title.Replace(item.ToString(), "");
            }
            return title.Trim();
        }
    }
}
