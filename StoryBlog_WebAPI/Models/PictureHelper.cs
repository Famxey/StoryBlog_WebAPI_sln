using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryBlog_WebAPI.Models
{
    public class PictureHelper
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string picClsTitle { get; set; }
        public string ImgFile { get; set; }
        public System.DateTime picCreateTime { get; set; }
        public Nullable<int> picHot { get; set; }
        public int PicClsID { get; set; }
        public string uAccount { get; set; }
        public string picDescribe { get; set; }

        public string picCreateTimeStr
        {
            get { return picCreateTime.ToString("yyyy/MM/dd/HH:mm:ss"); }
        }
    }
}