using AutoMapper;
using BoboModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeiSalonV2.Models
{
    public class AutoMapperConfiguration_Load
    {
             /// <summary>
        /// AutoMapper配置映射的初始化方法 一开始就调用了...
        /// </summary>
        public static void Configure()
        {
            Mapper.Initialize(opt => 
            {
                //opt.AddProfile<AllSalon>();
            });

        }
    }
}