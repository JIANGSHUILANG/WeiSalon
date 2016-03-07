using AutoMapper;
using Boboframework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoboModel
{
    public class BaseAutoMapper : Profile
    {
        protected override void Configure()
        {
            // AllSalon salon = AutoMapper.Mapper.Map<AllSalon>(salon_Model);    
            #region 将T_Salon映射为-->>AllSalon

            //CreateMap<T_Salon, AllSalon>()
            //    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.s_address))
            //    .ForMember(dest => dest.Cell, opt => opt.MapFrom(src => src.s_cell))
            //    .ForMember(dest => dest.Suid, opt => opt.MapFrom(src => src.s_uid))
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.s_status))
            //    .ForMember(dest => dest.SiKind, opt => opt.MapFrom(src => src.s_kind))
            //    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.s_price))
            //    .ForMember(dest => dest.Nickname, opt => opt.MapFrom(src => src.s_nickname))
            //    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.s_email))
            //    .ForMember(dest => dest.Pwd, opt => opt.MapFrom(src => src.s_pwd))
            //    .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.s_logo))
            //    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.s_date))
            //    .ForMember(dest => dest.LinkName, opt => opt.MapFrom(src => src.s_linkname))
            //    .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.s_summary))
            //    .ForMember(dest => dest.Lon, opt => opt.MapFrom(src => src.s_lon))
            //    .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => src.s_lat))
            //    .ForMember(dest => dest.Wxname, opt => opt.MapFrom(src => src.s_wxname))
            //    ;
            
            #endregion

            #region HariStyleList
            
            #endregion

            base.Configure();
        }
    }

    public class BaseDtoModel
    { 
    
    }
}
