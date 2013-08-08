using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogFun
{
    public class Post
    {
        public string Title { get; set; }

        public string Content { get; set; }
    }

    public class BlogIndexItem
    {
        public string Title { get; set; }
        public string URL { get; set; }
        public BlogIndexItem(string title, string url)
        {
            this.Title = title;
            this.URL = url;
        }
        public BlogIndexItem() { }
    }

    public class ContentSem
    {
        public ContentSem(string type, int index, int length, string content)
        {
            this.Type = type;
            this.Index = index;
            this.Length = length;
            this.Content = content;
        }
        public ContentSem()
        {

        }
        public string Content { get; set; }
        public string Type { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
    }
}
