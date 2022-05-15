using MongoDB.Driver;
using SpiderBot.Data.Entities;
using SpiderBot.Data.Interfaces;

namespace SpiderBot.Data.Repositories
{
    public class AnnouncementRepository : Repository<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(MongoClient mongoClient) : base(mongoClient) { }
        public AnnouncementRepository() { }
    }
}
