using Boboframework.Data;
using BoboModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface ICustomerManageService : IBaseService<t_customermanage>
    {
        /// <summary>
        /// 获取顾客项目的基本信息
        /// </summary>
        /// <param name="S_UID">沙龙ID</param>
        /// <returns></returns>
        IQueryable<CustomerManageInfo> GetCustomerInfo(string S_UID);
    }
    public class CustomerManageService : BaseService<t_customermanage>, ICustomerManageService
    {
        /// <summary>
        /// 获取顾客项目的基本信息
        /// </summary>
        /// <param name="S_UID">沙龙ID</param>
        /// <returns></returns>
        public IQueryable<CustomerManageInfo> GetCustomerInfo(string S_UID)
        {
            string sql = string.Format(@"select 
wm.wm_nickname as Customer_Name,
wm.wm_type as Customer_Come,
wm.wm_headimgurl as Customer_Log,
 wm.wm_cell as Customer_Cell,
 wm.wm_openid as OpenId,
 tc.cm_h_name as Attention_HairList,
 tc.cm_h_uid as Hair_Id,
 tc.cm_remarks1 as Customer_Remark,
 tc.cm_status  as Attention_Status,
tc.cm_id as Customer_Id
 from 
T_CustomerManage as tc
left join 
Wx_Member as wm
on  tc.cm_openid=wm_openid 
left join 
Wx_BindHair as wb
on wb.wb_openid=wm.wm_openid and tc.cm_h_uid=wb.wb_h_uid  where tc.cm_s_uid='{0}'", S_UID);
            return _db.Database.SqlQuery<CustomerManageInfo>(sql).AsQueryable();
        }
    }
}
