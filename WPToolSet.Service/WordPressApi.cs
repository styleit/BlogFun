using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using WPToolSet.Entity;

namespace WPToolSet.Service
{
    public class WordPressApi : XmlRpcClientProtocol
    {
        [XmlRpcMethod("wp.newPost")]
        public string WpNewPost(int blogID,string userName,string password,WordpressPost content)
        {
            return (string)this.Invoke("WpNewPost",new object[]{blogID,userName,password,content});
        }
    }
}
