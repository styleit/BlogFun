using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPToolSet.Entity;
using WPToolSet.Service;
using Log;

namespace BlogFun
{
    public class PubPost
    {
        private LogFlie publishLog = new LogFlie("pub_process_trace.log");
        private LogFlie hist = new LogFlie("hist.log");
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
                    try
                    {
                        SavePost(item.Title, item.Content);
                    }
                    catch (Exception ex)
                    {
                        publishLog.Add("Wil saving post "+item.Title+" meet issue: " + ex.ToString());
                        System.Threading.Thread.Sleep(10);
                        lock (queue)
                        {
                            queue.Enqueue(item);
                        }
                        throw;
                    }
                    
                }

            }
        }

        

        private void SavePost(string title, string postContent)
        {
            //title = BlogFunUtlity.filterTitle(title);
            //StreamWriter sw = new StreamWriter(title + ".html");
            //sw.Write(content);
            //sw.Close();
            WordPressApi WordPress = new WordPressApi();
            WordPress.Url = "http://www.zuihoude.com/xmlrpc.php";
            string user = "admin";
            string password = "6505237";
            WordpressPost content = new WordpressPost();

            content.post_title = title;
            content.post_name = title;
            content.post_content = postContent;
            //content.post_excerpt = "asdf";
            content.post_date = DateTime.Now;
            content.post_status = "draft";
            content.post_type = "post";

            //content.terms_names.category = new string[] { "Test" };

            //CustomField[] cf = { new CustomField { key = "test", value = "1231233" } };
            //content.custom_fields = cf;

            string PostID = WordPress.WpNewPost(0, user, password, content);
            publishLog.Add("Post published : " + PostID);
            hist.Add(title);
            Console.WriteLine("Post published : "+PostID);
            //Console.ReadLine();
        }
    }
}
