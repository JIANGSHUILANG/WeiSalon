/*----------------------------------------------------------------
// 莫迪思
// 版权所有。
//
// 文件名：Backflow.cs
// 功能描述：
// 
// 创建人  ： KK
// 创建标识： 2015/3/11 10:57:05
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

namespace BoboModel
{
    /// <summary>
    /// 支付订单表
    /// </summary>
    public class Order
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string Orid { get; set; }

        /// <summary>
        /// 沙龙ID
        /// </summary>
        public Guid? Suid { get; set; }

        /// <summary>
        /// 订单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 订单金额
        /// </summary>
        public double? Money { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public System.DateTime? Date { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public System.DateTime? Paydate { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int? Status { get; set; }
    }
}
