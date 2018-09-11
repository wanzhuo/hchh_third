using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

namespace ZRui.Web
{
    public static class DbContextExtention
    {
        public static T AddTo<T>(this DbContext context, T model) where T : class
        {
            context.Set<T>().Add(model);
            return model;
        }

        public static T GetSingle<T>(this DbContext context, int id) where T : EntityBase
            => context.Set<T>().Where(m => m.Id == id).FirstOrDefault();

        public static T GetSingleByCondition<T>(this DbContext context, System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : EntityBase
        {
            return context.Set<T>().Where(m => !m.IsDel)
                .Where(predicate)
                .FirstOrDefault();
        }

        public static T GetSingle<T>(this DbContext context, long id) where T : EntityBase<long>
            => context.Set<T>().Where(m => m.Id == id).FirstOrDefault();

        public static EntityEntry<T> Delete<T>(this DbContext context, int id) where T : EntityBase
        {
            var model = context.GetSingle<T>(id);
            if (model != null)
                return context.Set<T>().Remove(model);
            else
                return null;
        }

        public static EntityEntry<T> Delete<T>(this DbContext context, long id) where T : EntityBase<long>
        {
            var model = context.GetSingle<T>(id);
            if (model != null)
                return context.Set<T>().Remove(model);
            else
                return null;
        }

        public static IQueryable<T> Query<T>(this DbContext context) where T : class
            => context.Set<T>().AsQueryable();

        public static DbSet<T> DbSet<T>(this DbContext context) where T : class
            => context.Set<T>();
    }
}
