using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogFun
{
    public class PubPost
    {
        public void Process(object postItem)
        {
            Queue<Post> queue = (Queue<Post>)postItem;
            int queueCount = 0;
            
            while (true)
            {
                lock (queue)
                {
                    queueCount = queue.Count;
                }
                if (queueCount == 0)
                {

                }
                else
                {
                    Post item = null;
                    lock (queue)
                    {
                        item = queue.Dequeue();
                    }
                    SavePost(item.Title,item.Content);
                }

            }
        }

        

        private void SavePost(string title, string content)
        {
            title = BlogFunUtlity.filterTitle(title);
            StreamWriter sw = new StreamWriter(title + ".html");
            sw.Write(content);
            sw.Close();
        }
    }
}
