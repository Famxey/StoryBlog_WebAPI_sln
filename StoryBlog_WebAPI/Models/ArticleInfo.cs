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
    
    public partial class ArticleInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ArticleInfo()
        {
            this.ArtComment = new HashSet<ArtComment>();
        }
    
        public int ID { get; set; }
        public string artNo { get; set; }
        public string Title { get; set; }
        public string artContent { get; set; }
        public System.DateTime artCreateTime { get; set; }
        public int artClsID { get; set; }
        public string uAccount { get; set; }
        public int artAuthority { get; set; }
        public int artHot { get; set; }
        public int artComCnt { get; set; }
        public string artDigest { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArtComment> ArtComment { get; set; }
        public virtual ArticleClass ArticleClass { get; set; }
        public virtual UserInfo UserInfo { get; set; }
    }
}
