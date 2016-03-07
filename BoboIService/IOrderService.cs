/*----------------------------------------------------------------
// 莫迪思
// 版权所有。
//
// 文件名：IOrderService.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/3/6 14:04:40
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoboModel;
using System.Data;
using Boboframework;
using Boboframework.Data;
using System.Data.Entity;

namespace BoboIService
{
    public partial interface IOrderService : IBaseService<t_order>
    {
        /// <summary>
        /// 获取订单表
        /// </summary>
        /// <returns></returns>
        List<Order> GetOrderList(int pageindex, int pagesize, out int cou, out int pagcou);

        /// <summary>
        /// 添加分享 回流信息
        /// </summary>
        /// <param name="Ordermo">分享 回流信息</param>
        /// <returns></returns>
        int AddOrder(Order Ordermo);

        /// <summary>
        /// 获取订单表
        /// </summary>
        /// <returns></returns>
        Order GetOrderInfo(string OrderNo);

        /// <summary>
        /// 获取订单表
        /// </summary>
        /// <returns></returns>
        Order GetOrderInfo(int OrderID);

        /// <summary>
        /// 获取订单表
        /// </summary>
        /// <returns></returns>
        Order GetOrderInfo(Guid uid, DateTime dt);

        /// <summary>
        /// 修改订单表status
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        int UpdateOrderstatus(int id, int status);
    }
    public partial class OrderService : BaseService<t_order>, IOrderService
    {
        private readonly mysqkbobohairEntitesContainer _db = DbContextManager.DefaultInstance;
        private readonly IErrorService _err = new ErrorService();
        #region IOrderService 成员

        /// <summary>
        /// 获取订单表
        /// </summary>
        /// <returns></returns>
        public List<Order> GetOrderList(int pageindex, int pagesize, out int cou, out int pagcou)
        {
            var list = new PageOfItems<Order>() { PageNumber = pageindex, PageSize = pagesize };
            list.TotalItemCount = _db.t_order
                .OrderByDescending(a => a.o_date).Count();
            cou = list.TotalItemCount;
            pagcou = list.TotalPageCount;
            var beginnum = (pageindex - 1) * pagesize;

            var info = new List<Order>();
            try
            {
                info = _db.t_order
                .OrderByDescending(a => a.o_date)
                                .Skip(beginnum)
                                .Take(pagesize)
                              .Select(a => new Order()
                              {
                                  Id = a.o_id,
                                  Date = a.o_date,
                                  Money = a.o_money,
                                  Name = a.o_name,
                                  Orid = a.o_orid,
                                  Paydate = a.o_paydate,
                                  Status = a.o_status,
                                  Suid = a.o_s_uid
                              }).ToList();
            }
            catch (Exception ex)
            {

                _err.AddError("GetOrderList", ex.Message);
            }
            return info;
        }

        /// <summary>
        /// 添加分享 回流信息
        /// </summary>
        /// <param name="Ordermo">分享 回流信息</param>
        /// <returns></returns>
        public int AddOrder(Order Ordermo)
        {
            var model = new t_order()
            {
                o_date = Ordermo.Date,
                o_money = Ordermo.Money,
                o_name = Ordermo.Name,
                o_orid = Ordermo.Orid,
                o_paydate = Ordermo.Paydate,
                o_status = Ordermo.Status,
                o_s_uid = Ordermo.Suid,
            };
            try
            {
                _db.t_order.Add(model);
                return _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.t_order.Remove(model);
                _err.AddError("AddOrder", ex.Message);
            }
            return 0;
        }

        /// <summary>
        /// 获取订单表
        /// </summary>
        /// <returns></returns>
        public Order GetOrderInfo(int OrderID)
        {
            var info = new List<Order>();
            try
            {
                info = _db.t_order
                .OrderByDescending(a => a.o_date)
                              .Where(a => a.o_id == OrderID)
                              .Select(a => new Order()
                              {
                                  Id = a.o_id,
                                  Date = a.o_date,
                                  Money = a.o_money,
                                  Name = a.o_name,
                                  Orid = a.o_orid,
                                  Paydate = a.o_paydate,
                                  Status = a.o_status,
                                  Suid = a.o_s_uid
                              }).ToList();
            }
            catch (Exception ex)
            {
                _err.AddError("GetOrderInfo", ex.Message);
            }
            return info.FirstOrDefault();
        }

        /// <summary>
        /// 获取订单表
        /// </summary>
        /// <returns></returns>
        public Order GetOrderInfo(string OrderNo)
        {
            var info = new List<Order>();
            try
            {
                info = _db.t_order
                .OrderByDescending(a => a.o_date)
                              .Where(a => a.o_orid == OrderNo)
                              .Select(a => new Order()
                              {
                                  Id = a.o_id,
                                  Date = a.o_date,
                                  Money = a.o_money,
                                  Name = a.o_name,
                                  Orid = a.o_orid,
                                  Paydate = a.o_paydate,
                                  Status = a.o_status,
                                  Suid = a.o_s_uid
                              }).ToList();
            }
            catch (Exception ex)
            {
                _err.AddError("GetOrderInfo", ex.Message);
            }
            return info.FirstOrDefault();
        }

        /// <summary>
        /// 获取订单表
        /// </summary>
        /// <returns></returns>
        public Order GetOrderInfo(Guid uid, DateTime dt)
        {
            var info = new List<Order>();
            try
            {
                info = _db.t_order
                .OrderByDescending(a => a.o_date)
                              .Where(a => a.o_s_uid == uid && a.o_status == 0 && a.o_date > dt)
                              .Select(a => new Order()
                              {
                                  Id = a.o_id,
                                  Date = a.o_date,
                                  Money = a.o_money,
                                  Name = a.o_name,
                                  Orid = a.o_orid,
                                  Paydate = a.o_paydate,
                                  Status = a.o_status,
                                  Suid = a.o_s_uid
                              }).ToList();
            }
            catch (Exception ex)
            {
                _err.AddError("GetOrderInfo", ex.Message);
            }
            return info.FirstOrDefault();
        }

        /// <summary>
        /// 修改订单表status
        /// </summary>
        /// <param name="OrderNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int UpdateOrderstatus(int id, int status)
        {
            int count = 0;
            var model = _db.t_order.Find(id);
            try
            {
                model.o_status = status;
                model.o_paydate = DateTime.Now.AddSeconds(1);
                _db.Entry(model).State = EntityState.Modified;
                count = _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _db.Entry(model).Reload();
                _err.AddError("UpdateOrderstatus", ex.Message);
            }
            return count;
        }
        #endregion

    }




    public partial interface IOrderService : IBaseService<t_order>
    {
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="typetodayorall">1:今日订单  其它：所有订单</param>
        /// <returns></returns>
        IQueryable<OrderManageInfo> GetOrderManage(string SalonID, int typetodayorall);
    }

    public partial class OrderService : BaseService<t_order>, IOrderService
    {
        /// <summary>
        /// 获取订单信息
        /// </summary>
        /// <param name="typetodayorall">1:今日订单  其它：所有订单</param>
        /// <returns></returns>
        public IQueryable<OrderManageInfo> GetOrderManage(string SalonID, int typetodayorall)
        {
            StringBuilder sb = new StringBuilder();
            string temp = string.Empty;
            if (typetodayorall == 1)  //今日订单
            {
                temp = " and DATEDIFF(DAY,wo_date,getdate())=0";
            }
            sb.AppendFormat(@"select
T_S.shs_name as hairname,
W_X.wm_nickname as customername,
W_O.wo_bookingdate as bookingdate,
W_O.wo_date as ordertime,
W_O.wo_price as ordermoney,
W_C.wc_content as comment,
W_O.wo_id
 from (select * from Wx_Order where wo_s_uid='{0}' {1}  and wo_orderstatus=2 ) as W_O
 left join
		t_salinhairstylist as T_S
		on W_O.wo_h_uid=T_S.shs_h_uid
left join
         Wx_Member as W_X			--沙龙顾客
           on W_O.wo_openid=W_X.wm_openid 
left join
	(select * from	Wx_Comment where wc_status=1) as W_C 
			on W_O.wo_id=W_C.wc_wo_id
     order by W_O.wo_bookingdate desc", SalonID, temp);
            //店名 发型师名 顾客名字 预约时间 下单时间 金额 评论
            return this._db.Database.SqlQuery<OrderManageInfo>(sb.ToString()).AsQueryable();
        }
    }
}
