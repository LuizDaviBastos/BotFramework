using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SpiderBot.Data.Entities
{
    public class Announcement : EntityBase
    {

        [BsonRequired]
        public string Title { set; get; }

        [BsonRequired]
        public string Price { set; get; }
        
        [BsonRepresentation(BsonType.Boolean)]
        [BsonRequired]
        public bool Status { set; get; }

        [BsonRequired]
        public string Access { set; get; }
    }
}
