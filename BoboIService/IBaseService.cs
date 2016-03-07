using BoboCommon;
using Boboframework;
using Boboframework.Data;
using BoboModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace BoboIService
{
    public interface IBaseService<T> where T : class,new()
    {
        
        #region 查询方法

        /// <summary>
        /// 查询，根据条件查询
        /// </summary>
        /// <param name="whereLambda">条件</param>
        /// <returns></returns>
        IQueryable<T> LoadEntities(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// 查询，根据条件查询
        /// </summary>
        /// <param name="whereLambda">条件</param>
        /// <returns></returns>
        IQueryable<Dto> LoadEntities<Dto>(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda) where Dto : BaseDtoModel;

        /// <summary>
        /// 查询分页，返回总页数
        /// </summary>
        /// <typeparam name="s">排序字段的类型<!--orderbyLambad使用--></typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="totalCount">总页数</param>
        /// <param name="whereLambda">查询条件</param>
        /// <param name="orderbyLambad">排序条件</param>
        /// <param name="isAsc">正序:true  倒序:false</param>
        /// <returns></returns>
        IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, out int totalCount, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderbyLambad, bool isAsc);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="s">排序字段</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="whereLambda">条件</param>
        /// <param name="orderbyLambad">排序条件</param>
        /// <param name="isAsc">正序:true  倒序:false</param>
        /// <returns></returns>
        IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderbyLambad, bool isAsc);


        /// <summary>
        /// 分页查询 
        /// </summary>
        /// <typeparam name="s">排序字段</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="whereLambda">条件</param>
        /// <param name="orderbyLambad">排序条件</param>
        /// <param name="isAsc">默认正序 正序:true  倒序:false </param>
        /// <returns></returns>
        PageOfItems<T> PageOfItemsLoadPageEntites<s>(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderbyLambad = null, bool isAsc = true);

       /// <summary>
       /// 传入集合分页
       /// </summary>
       /// <typeparam name="S"></typeparam>
       /// <typeparam name="OrderByType"></typeparam>
       /// <param name="pageIndex"></param>
       /// <param name="pageSize"></param>
       /// <param name="list">传入的集合</param>
       /// <param name="whereLambda"></param>
       /// <param name="orderbyLambad"></param>
       /// <returns></returns>
        PageOfItems<S> PageOfItemBySelfModel<S, OrderByType>(int pageIndex, int pageSize, IQueryable<S> list, System.Linq.Expressions.Expression<Func<S, bool>> whereLambda, System.Linq.Expressions.Expression<Func<S, OrderByType>> orderbyLambad = null, bool isAsc = true);
        #endregion
        bool DeleteEntity(T entity);
        bool UpdateEntity(T entity);
        T AddEntity(T entity);
    }

    /// <summary>
    /// 封装的是业务层中公共的代码（CURD）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseService<T> : IBaseService<T> where T : class,new()
    {
        protected readonly mysqkbobohairEntitesContainer _db = DbContextManager.DefaultInstance;




        #region EF
        //private mysqkbobohairEntitesContainer _db
        //{
        //    get
        //    {
        //        #region 保证EF上下文线程内唯一 ..

        //        object dbcontext = (DbContext)CallContext.GetData("dbContext");
        //        if (dbcontext == null)
        //        {
        //            mysqkbobohairEntitesContainer ef = DbContextManager.DefaultInstance;
        //            CallContext.SetData("dbContext", ef);
        //        }
        //        return (mysqkbobohairEntitesContainer)dbcontext;
        //        #endregion
        //    }
        //} 
        #endregion

        #region 查询方法

        /// <summary>
        /// 查询，根据条件查询所有
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <returns></returns>
        public IQueryable<T> LoadEntities(System.Linq.Expressions.Expression<Func<T, bool>> whereLambda)
        {
            return _db.Set<T>().Where<T>(whereLambda);
        }

        public IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, out int totalCount, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderbyLambad, bool isAsc)
        {
            var temp = _db.Set<T>().Where<T>(whereLambda);
            totalCount = temp.Count();
            if (isAsc)//升序
            {
                temp = temp.OrderBy<T, s>(orderbyLambad).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }
            else
            {
                temp = temp.OrderByDescending<T, s>(orderbyLambad).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }
            return temp;
        }

        public IQueryable<T> LoadPageEntities<s>(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderbyLambad, bool isAsc)
        {
            var temp = _db.Set<T>().Where<T>(whereLambda);

            if (isAsc)//升序
            {
                temp = temp.OrderBy<T, s>(orderbyLambad).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }
            else
            {
                temp = temp.OrderByDescending<T, s>(orderbyLambad).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }
            return temp;
        }


        /// <summary>
        /// 分页查询 
        /// </summary>
        /// <typeparam name="s">排序字段</typeparam>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页尺寸</param>
        /// <param name="whereLambda">条件</param>
        /// <param name="orderbyLambad">排序条件</param>
        /// <param name="isAsc">正序:true  倒序:false </param>
        /// <returns></returns>
        public PageOfItems<T> PageOfItemsLoadPageEntites<s>(int pageIndex, int pageSize, System.Linq.Expressions.Expression<Func<T, bool>> whereLambda, System.Linq.Expressions.Expression<Func<T, s>> orderbyLambad = null, bool isAsc = true)
        {

            var info = _db.Set<T>().Where<T>(whereLambda);

            var list = new PageOfItems<T>()
            {
                PageNumber = pageIndex,
                PageSize = pageSize,
                TotalItemCount = info.Count()
            };

            IQueryable<T> orderList = null;
            if (isAsc)
            {
                orderList = info.OrderBy<T, s>(orderbyLambad).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }
            else
            {
                orderList = info.OrderByDescending<T, s>(orderbyLambad).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
            }

            list.AddRange(orderList.ToList());
            return list;
        }

        /// <summary>
        /// 传入集合分页
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="OrderByType"></typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="list">传入的集合</param>
        /// <param name="whereLambda"></param>
        /// <param name="orderbyLambad"></param>
        /// <returns></returns>
        public PageOfItems<S> PageOfItemBySelfModel<S, OrderByType>(int pageIndex, int pageSize, IQueryable<S> list, System.Linq.Expressions.Expression<Func<S, bool>> whereLambda, System.Linq.Expressions.Expression<Func<S, OrderByType>> orderbyLambad = null,bool isAsc=true)
        {
            var info = list.Where<S>(whereLambda);

            var list_return = new PageOfItems<S>()
            {
                PageNumber = pageIndex,
                PageSize = pageSize,
                TotalItemCount = info.Count()
            };
            IQueryable<S> orderList = null;
            if (orderbyLambad != null)
            {
                if (isAsc)
                {
                    orderList = info.OrderBy<S, OrderByType>(orderbyLambad).Skip<S>((pageIndex - 1) * pageSize).Take<S>(pageSize);
                }
                else
                {
                    orderList = info.OrderByDescending<S, OrderByType>(orderbyLambad).Skip<S>((pageIndex - 1) * pageSize).Take<S>(pageSize);
                }
            }
            else
            {
                orderList = info.Skip<S>((pageIndex - 1) * pageSize).Take<S>(pageSize);
            }
            list_return.AddRange(orderList.ToList());
            return list_return;
        }

        #endregion

        public bool DeleteEntity(T entity)
        {
            _db.Entry<T>(entity).State = EntityState.Deleted;
            return _db.SaveChanges() > 0;
        }

        public bool UpdateEntity(T entity)
        {
            _db.Entry<T>(entity).State = EntityState.Modified;
            return _db.SaveChanges() > 0;
        }

        public T AddEntity(T entity)
        {
            _db.Set<T>().Add(entity);
            return _db.SaveChanges() > 0 ? entity : null;
        }

        protected virtual bool MapperToDTO()
        {
            return false;
        }

        public IQueryable<Dto> LoadEntities<Dto>(Expression<Func<T, bool>> whereLambda) where Dto : BaseDtoModel
        {
            //var dtomodel = MapperToDTO<Dto>();
            //IQueryable<Dto> list = null;
            //if (dtomodel != null)
            //{
            bool b = MapperToDTO();
            var list = _db.Set<T>().Project().To<Dto>();
            //}
            return list;
        }

        public event Action MappingHandler;
    }
}
