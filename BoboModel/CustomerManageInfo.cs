using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
  public  class CustomerManageInfo : BaseDtoModel
    {
      public string OpenId { get; set; }
      public string Hair_Id { get; set; }
      public string Customer_Name { get; set; }
      public string Customer_Log { get; set; }
      public int? Customer_Id { get; set; }
      /// <summary>
      /// 顾客来源
      /// </summary>
      public int? Customer_Come { get; set; }
      /// <summary>
      /// 关注的发型师
      /// </summary>
      public string Attention_HairList { get; set; }
      public string Customer_Cell { get; set; }
      /// <summary>
      /// 顾客生日等备注信息
      /// </summary>
      public string Customer_Remark { get; set; }
      /// <summary>
      /// 关注状态   0 取消   1 关注
      /// </summary>
      public int? Attention_Status { get; set; }
      
    }
}
