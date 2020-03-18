using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoryBlog_WebAPI.Models
{
    public partial class UserInfoHelper:FlagHelper
    {
        public string Account { get; set; }
        public string Guid { get; set; }
        public string NickName { get; set; }
        public string PassWord { get; set; }
        public string Picture { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public Nullable<int> Age { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public string Describe { get; set; }
        public string Introduce { get; set; }

        public string BGPicture { get; set; }
    }
}