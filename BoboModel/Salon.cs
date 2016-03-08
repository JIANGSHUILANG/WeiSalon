using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    /// <summary>
    /// 沙龙表 
    /// </summary>
    public class SalonSimple  //: BaseAutoMapper
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Uid { get; set; }

        /// <summary>
        /// 登陆
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Cell { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 开通到期时间
        /// </summary>
        public DateTime? Opendate { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public int? Kind { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 沙龙地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Lon { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string Lat { get; set; }

        /// <summary>
        /// 沙龙微信号名称
        /// </summary>
        public string Wxname { get; set; }

        /// <summary>
        /// 沙龙剪发价格
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// 营业时间
        /// </summary>
        public string Businessdate { get; set; }

        /// <summary>
        /// 销售人员姓名
        /// </summary>
        public string SaleName { get; set; }

        /// <summary>
        /// 销售人ID
        /// </summary>
        public int? Sale_Id { get; set; }

        /// <summary>
        /// 最后操作时间
        /// </summary>
        public Nullable<DateTime> LastTime { get; set; }

        public string Recommend { get; set; }

    }

    /// <summary>
    /// 沙龙图片信息表 
    /// </summary>
    public class AllSalon : SalonSimple
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 沙龙ID
        /// </summary>
        public Guid Suid { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 类型  1 营业执照 2 正门 3 环境照 4 发型价格
        /// </summary>
        public int? SiKind { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Sistatus { get; set; }

        //营业时间
        public string Businessdate { get; set; }

        //沙龙公告
        public string noticecontent { get; set; }

        //发型师背景图片状态
        public string ImageUrl { get; set; }
    }

    public class OneSalonInfo
    {

        /// <summary>
        /// ID
        /// </summary>
        public Guid Uid { get; set; }


        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Cell { get; set; }


        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public int? Kind { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 沙龙地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string LinkName { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Summary { get; set; }


        /// <summary>
        /// 沙龙微信号名称
        /// </summary>
        public string Wxname { get; set; }

        /// <summary>
        /// 沙龙剪发价格
        /// </summary>
        public string Price { get; set; }


        /// <summary>
        /// 图片
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 类型  1 营业执照 2 正门 3 环境照 4 发型价格
        /// </summary>
        public int? SiKind { get; set; }

    }
}
