﻿namespace QuizHut.DAL.Repositories
{
    using Microsoft.EntityFrameworkCore;

    using QuizHut.DAL.Common.Models;
    using QuizHut.DAL.EntityFramework;
    using QuizHut.DLL.Repositories.Contracts;

    public class EfDeletableEntityRepository<TEntity> : EfRepository<TEntity>, IDeletableEntityRepository<TEntity>
        where TEntity : class, IDeletableEntity
    {
        public EfDeletableEntityRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public override IQueryable<TEntity> All() => base.All().Where(x => !x.IsDeleted);

        public override IQueryable<TEntity> AllAsNoTracking() => base.AllAsNoTracking().Where(x => !x.IsDeleted);

        public IQueryable<TEntity> AllWithDeleted() => base.All().IgnoreQueryFilters();

        public IQueryable<TEntity> AllAsNoTrackingWithDeleted() => base.AllAsNoTracking().IgnoreQueryFilters();

        public async Task<TEntity> GetByIdWithDeletedAsync(params object[] id)
        {
            var getByIdPredicate = EfExpressionHelper.BuildByIdPredicate<TEntity>(Context, id);
            return await AllWithDeleted().FirstOrDefaultAsync(getByIdPredicate);
        }

        public void HardDelete(TEntity entity) => base.Delete(entity);

        public void Undelete(TEntity entity)
        {
            entity.IsDeleted = false;
            entity.DeletedOn = null;
            Update(entity);
        }

        public override void Delete(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            Update(entity);
        }
    }
}