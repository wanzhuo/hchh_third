using System;
using System.Collections.Generic;
using System.Text;

namespace ZRui.Web
{
    public interface IEntityBase<IdType>  where IdType : struct
    {
        /// <summary>
        /// 编号
        /// </summary>
        IdType Id { get; set; }
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        bool IsDel { get; set; }
    }

    public abstract class EntityBase<IdType> : IEntityBase<IdType> where IdType : struct
    {
        /// <summary>
        /// 编号
        /// </summary>
        public IdType Id { get; set; }
        /// <summary>
        /// 是否逻辑删除
        /// </summary>
        public bool IsDel { get; set; }
    }

    public abstract class EntityBase : EntityBase<int>, IEntityBase<int>
    {

    }
}
