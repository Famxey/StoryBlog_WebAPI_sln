using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryBlog_WebAPI.Models
{
    public class ArticleCommentHelper
    {
        public int ID { get; set; }
        public string artCmContent { get; set; }
        public System.DateTime artCmCreateTime { get; set; }
        public string uAccount { get; set; }
        public string NickName { get; set; }
        public int artID { get; set; }
        public string uPicture { get; set; }

        public string artCreateTimeA
        {
            get { return artCmCreateTime.ToString("yyyy/MM/dd/HH:mm:ss"); }
        }
    }
}