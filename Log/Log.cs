using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Log
{
    public class LogFlie
    {
        private string LogPath = "trace.log";
        StreamWriter logStream = null;
        public LogFlie(string path)
        {
            this.LogPath = path;
            logStream = new StreamWriter(this.LogPath,true);
        }
        public LogFlie() {
            logStream = new StreamWriter(this.LogPath,true);
        }

        public void Add(string logInfo)
        {
            logStream.WriteLine(logInfo);
            logStream.Flush();
        }
    }
}
