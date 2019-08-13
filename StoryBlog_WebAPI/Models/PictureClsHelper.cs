using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryBlog_WebAPI.Models
{
    public class PictureClsHelper
    {
        public int ID { get; set; }
        public string picClsTitle { get; set; }
        public string CoverFile { get; set; }
        public string uAccount { get; set; }
        public int picClsAuthority { get; set; }
        public string picClsDescribe { get; set; }
        public int picClsPicCnt { get; set; }
        public System.DateTime picClsCreateTime { get; set; }

        public string picClsCreateTimeA
        {
            get { return picClsCreateTime.ToString("yyyy/MM/dd/HH:mm:ss"); }
        }
    }
}