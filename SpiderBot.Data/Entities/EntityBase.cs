using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpiderBot.Data.Entities
{
    public class EntityBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { set; get; }
    }
}
