//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace StoryBlog_WebAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AttentionInfo
    {
        public int ID { get; set; }
        public string uAccount { get; set; }
        public string attenUser { get; set; }
    
        public virtual UserInfo UserInfo { get; set; }
    }
}
