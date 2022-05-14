using MongoDB.Bson.Serialization.Attributes;

namespace SpiderBot.Data.Entities
{
    public class Title : EntityBase
    {
        [BsonRequired]
        public string Name { set; get; }
    }
}
