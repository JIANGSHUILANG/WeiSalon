//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Boboframework.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class wx_comment
    {
        public int wc_id { get; set; }
        public string wc_openid { get; set; }
        public Nullable<System.Guid> wc_h_uid { get; set; }
        public string wc_content { get; set; }
        public Nullable<System.DateTime> wc_date { get; set; }
        public Nullable<int> wc_wo_id { get; set; }
        public Nullable<int> wc_score { get; set; }
        public Nullable<int> wc_status { get; set; }
        public string wc_image { get; set; }
    }
}