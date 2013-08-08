using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace WPToolSet.Entity
{
    public class Site
    {
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string Feed { get; set; }
        public string SiteMap { get; set; }
    }
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct CustomField
    {
        public string key;
        public string value;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Enclosure
    {
        public string url;
        public int lenght;
        public string type;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct Term
    {
        public string[] category;
        public string[] post_tag;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct WordpressPost
    {
        /// <summary>
        /// 'post' | 'page' | 'link' 
        /// </summary>
        public string post_type;
        /// <summary>
        /// 'draft' | 'publish' | 'pending'| 'future' | 'private' | custom registered status
        /// </summary>
        public string post_status;
        /// <summary>
        /// The title of your post
        /// </summary>
        public string post_title;
        public int post_author;
        public string post_excerpt;
        public string post_content;
        public DateTime post_date;
        public string post_format;
        /// <summary>
        /// Post nicky link
        /// </summary>
        public string post_name;
        public string post_password;
        public string comment_status;
        public string ping_status;
        /// <summary>
        /// Always Top
        /// </summary>
        public bool sticky;
        public int post_thumbnail;
        public int post_parent;
        public CustomField[] custom_fields;
        public Term terms;
        public Term terms_names;
        public Enclosure enclosure;
    }

}
