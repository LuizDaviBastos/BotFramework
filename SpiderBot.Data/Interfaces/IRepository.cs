namespace SpiderBot.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> Get();
        Task<TEntity> Get(string id);
        Task Delete(string id);
        Task Update(string id, TEntity entity);
        Task Insert(TEntity entity);
    }
}
