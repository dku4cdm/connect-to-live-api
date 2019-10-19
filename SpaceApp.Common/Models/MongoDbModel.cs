using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace SpaceApp.Common.Models
{
    public class MongoDbModel
    {
        protected MongoDbModel()
        {
            this.Created = DateTime.UtcNow;
        }

        protected MongoDbModel(Guid id)
        {
            this.Id = id;
        }

        [BsonId(IdGenerator = typeof(CombGuidGenerator))]
        public Guid Id { get; set; }

        [BsonElement]
        public DateTime Created { get; set; }

        [BsonElement]
        public DateTime Updated { get; set; }
    }
}
