using MongoDB.Driver;
using SpiderBot.Data.Entities;
using SpiderBot.Data.Interfaces;

namespace SpiderBot.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly IMongoDatabase db;
        protected readonly IMongoCollection<TEntity> collection;

        public Repository(MongoClient mongoClient)
        {
            db = mongoClient.GetDatabase("MongoSpiderDb");
            collection = db.GetCollection<TEntity>(nameof(TEntity));
        }

        public Repository() { }


        public virtual async Task Delete(string id)
        {
            await collection.DeleteOneAsync(x => x.Id == id);
        }

        public virtual async Task<IEnumerable<TEntity>> Get()
        {
            return (await collection.FindAsync(x => true)).ToList();
        }

        public virtual async Task<TEntity> Get(string id)
        {
            return (await collection.FindAsync(x => x.Id == id)).FirstOrDefault();
        }

        public virtual async Task Insert(TEntity entity)
        {
            await collection.InsertOneAsync(entity);
        }

        public virtual async Task Update(string id, TEntity entity)
        {
            await collection.ReplaceOneAsync(x => x.Id == id, entity);
        }
    }
}
