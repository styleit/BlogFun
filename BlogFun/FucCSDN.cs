using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Log;

namespace BlogFun
{
    public class FucCSDN
    {
        private string baseUrl = "http://blog.csdn.net/newest.html?page={0}";
        private Regex regIndex = new Regex("<div class=\"blog_list\">[\\d\\D]*?<h1>[\\d\\D]*?<a name[\\d\\D]*?href=\"(.*?)\"[\\d\\D]*?>(.*?)</a>");
        private Regex regContent = new Regex("id=\"article_content\"[\\d\\D]*?>([\\d\\D]*?)<div class=\"share_buttons\"");
        private Regex regImage = new Regex("<img.*?src=\"(.*?)\"");
        private Regex regCode = new Regex("<pre name=\"code\" class=\"(.*?)\">([\\d\\D]*?)</pre>");
        private Queue<Post> csdnPostQueue = null;
        private LogFlie processCsdnLog = new LogFlie("csdn_process_trace.log");
        private StreamReader hist = new StreamReader("hist.txt");
        string[] history;

        public void Process(object queue)
        {
            csdnPostQueue = (Queue<Post>)queue;
            string content = BlogFunUtlity.GetURLContents(string.Format(baseUrl,1));
            MatchCollection mc = regIndex.Matches(content);
            foreach (Match item in mc)
            {
                Console.WriteLine("Enqueue one item.");
                GetPostList(item.Groups[1].Value);
            }
        }

        private void GetPostList(string url)
        {
            Console.WriteLine("Processing {0}",url);
            history = hist.ReadToEnd().Split('\n');
            int detailIndex = url.IndexOf("details");
            string homePageUrl = url.Substring(0, detailIndex);
            string firstList = homePageUrl + "list/{0}";
            int maxPage = 0;

            Regex regPage = new Regex("<a href=.*?list/(\\d+)\">.*?</a>");
            string content = BlogFunUtlity.GetURLContents(string.Format(firstList, 1));
            MatchCollection mc = regPage.Matches(content);
            if (mc.Count == 0)
            {
                maxPage = 0;
            }
            else
            {
                maxPage = int.Parse(mc[mc.Count - 1].Groups[1].Value);
            }

            Regex regItemInPage = new Regex("<div class=\"article_title\">[\\d\\D]*?<span class=\"link_title\">[\\d\\D]*?href=\"(.*?)\">([\\d\\D]*?)</a>");
            for (int i = 0; i < maxPage; i++)
            {
                string pageContent = BlogFunUtlity.GetURLContents(string.Format(firstList, i + 1));
                MatchCollection itemsInPage = regItemInPage.Matches(pageContent);
                foreach (Match item in itemsInPage)
                {
                    string title = item.Groups[2].Value.Trim();
                    if (title.Contains("font"))
                    {
                        string[] TmpTitle = title.Split('>');
                        title = TmpTitle[TmpTitle.Length - 1];
                    }
                    CheckItem(new BlogIndexItem(title, "http://blog.csdn.net" + item.Groups[1].Value.Trim()));
                }
            }
        }

        private bool CheckHistory(BlogIndexItem item)
        {
            foreach (string title in history)
            {
                if (title == item.Title)
                {
                    return true;
                }
            }
            
            return false;
        }

        private void CheckItem(BlogIndexItem item)
        {
            if (!CheckHistory(item))
            {
                processContent(item);
            }
        }

        private void processContent(BlogIndexItem item)
        {
            string content = BlogFunUtlity.GetURLContents(item.URL);

            Match artical = regContent.Match(content);
            string result = artical.Groups[1].Value;

            List<ContentSem> markList = new List<ContentSem>();

            MatchCollection mc = regImage.Matches(result);
            foreach (Match imgItem in mc)
            {
                if (imgItem.Groups[1].Value.StartsWith("http:"))
                {
                    ContentSem cs = new ContentSem("img", imgItem.Groups[1].Index, imgItem.Groups[1].Length, imgItem.Groups[1].Value);
                    markList.Add(cs);
                }
            }

            mc = regCode.Matches(result);
            foreach (Match codeItem in mc)
            {
                ContentSem cs = new ContentSem("code", codeItem.Groups[0].Index, codeItem.Groups[0].Length, codeItem.Groups[0].Value);
                markList.Add(cs);
            }

            StringBuilder buffer = new StringBuilder();
            if (markList.Count > 0)
            {
                IEnumerable<ContentSem> orderList = markList.OrderBy(c => c.Index);
                ProcessImageCode(orderList);

                int index = 0;
                foreach (var listItem in orderList)
                {
                    buffer.Append(result.Substring(index, listItem.Index - index));
                    buffer.Append(listItem.Content);
                    index = listItem.Index + listItem.Length;
                }
                buffer.Append(result.Substring(index, result.Length - index));
            }
            else
            {
                buffer.Append(result);
            }

            string PostContent = buffer.ToString();
            Post CsdnPost = new Post();
            CsdnPost.Title = item.Title;
            CsdnPost.Content = PostContent;

            lock (csdnPostQueue)
            {
                csdnPostQueue.Enqueue(CsdnPost);
            }

            Console.WriteLine("Processing {0} Done.", item.URL);

        }

        #region Process post details
        private void ProcessImageCode(IEnumerable<ContentSem> markList)
        {
            foreach (ContentSem semItem in markList)
            {
                if (semItem.Type == "img")
                {
                   processImage(semItem); 
                }
                if (semItem.Type == "code")
                {
                   processCode(semItem);
                }
            }
        }

        private void processImage(ContentSem sem)
        {
            string uploadImgCMDPattern = "netdisk /e \"upload \\\"{0}\\\" \\app\\PublicFiles\\img-zuihoude\\{1}\\{2}\"";
            string filename = string.Empty;

            if (sem.Content.Contains("?") && sem.Content.StartsWith("http://img.blog.csdn.net/"))
            {
                sem.Content = sem.Content.Split('?')[0];
            }

            int fileNameIndex = sem.Content.Split('/').Length;
            filename = sem.Content.Split('/')[fileNameIndex - 1];
            if (sem.Content.StartsWith("http://img.blog.csdn.net/"))
            {
                fileNameIndex = sem.Content.Split('/').Length;
                filename = sem.Content.Split('/')[fileNameIndex - 1];
                filename = filename + ".jpg";
            }
            if (File.Exists(filename))
            {
                string imageOnPost = "/{0}/{1}/{2}";
                sem.Content = string.Format(imageOnPost, DateTime.Now.Year, DateTime.Now.Month, filename);
            }
            else
            {
                try
                {
                    HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(sem.Content);
                    HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse();
                    if (httpRes.StatusCode == HttpStatusCode.NotFound)
                    {
                        processCsdnLog.Add(string.Format("Image {0} return 404 Not Found.", sem.Content));
                        Console.WriteLine("Image {0} return 404 Not Found.", sem.Content);
                    }
                    else if (httpRes.StatusCode == HttpStatusCode.Forbidden)
                    {
                        processCsdnLog.Add(string.Format("Image {0} return  403 Forbidden.", sem.Content));
                        Console.WriteLine("Image {0} return 403 Forbidden.", sem.Content);
                    }
                    else
                    {
                        Stream responseStream = httpRes.GetResponseStream();
                        filename = BlogFunUtlity.filterTitle(filename);
                        Console.WriteLine("Will Save File : {0}", filename);
                        FileStream writer = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                        byte[] buffer = new byte[1024];
                        int count = 0;
                        while ((count = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, count);
                        }
                        writer.Close();
                        responseStream.Close();

                        string imageOnPost = "/{0}/{1}/{2}";
                        sem.Content = string.Format(imageOnPost, DateTime.Now.Year, DateTime.Now.Month, filename);

                        filename = System.Environment.CurrentDirectory + "\\" + filename;
                        string cmd = string.Format(uploadImgCMDPattern, filename, DateTime.Now.Year, DateTime.Now.Month);
                        processCsdnLog.Add(string.Format("Will execute command : {0}",cmd));
                        Console.WriteLine("Will execute command : {0}", cmd);
                        BlogFunUtlity.ExecuteCmd(cmd);

                    }
                }
                catch (Exception ex)
                {
                    processCsdnLog.Add(sem.Content + " meets issue: " +ex.ToString());
                    Console.WriteLine(sem.Content + " meets issue: " +ex.ToString());
                }
                
            }
        }

        private void processCode(ContentSem sem)
        {
            string content = HttpUtility.HtmlDecode(sem.Content);
            Match code = regCode.Match(content);
            string codePatern = "[{0}]{1}[/{2}]";
            sem.Content = string.Format(codePatern, code.Groups[1].Value, code.Groups[2].Value, code.Groups[1].Value);
        }
        #endregion
    }
}
