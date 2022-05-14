using MongoDB.Bson.Serialization.Attributes;

namespace SpiderBot.Data.Entities
{
    public class Access : EntityBase
    {
        [BsonRequired]
        public string Name { get; set; }
    }
}
