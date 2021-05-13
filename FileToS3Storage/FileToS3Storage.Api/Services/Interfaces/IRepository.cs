using System.Collections.Generic;

namespace FileToS3Storage.Api.Services.Interfaces
{
    public interface IRepository<TId, TEntity> 
        where TId : struct 
        where TEntity : class
    {
        TEntity Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        TEntity GetById(TId id);
        IList<TEntity> GetAll();
    }
}