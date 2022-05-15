using MongoDB.Driver;
using SpiderBot.Data.Entities;
using SpiderBot.Data.Interfaces;

namespace SpiderBot.Data.Repositories
{
    public class TitleRepository : Repository<Title>, ITitleRepository
    {
        public TitleRepository() { }
        public TitleRepository(MongoClient mongoClient) : base(mongoClient) { }

        public override Task<IEnumerable<Title>> Get()
        {
            return Task.Run(() =>
            {
                return new List<Title>()
                {
                    new Title
                    {
                        Name = "Amazon"
                    },
                    new Title
                    {
                        Name = "Netflix"
                    },
                    new Title
                    {
                        Name = "Disney"
                    },
                    new Title
                    {
                        Name = "HBO Max"
                    }
                }.AsEnumerable();
            });
        }

    }
}
