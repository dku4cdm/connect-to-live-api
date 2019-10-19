using MongoDB.Bson.Serialization.Attributes;
using SpaceApp.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceApp.Common.Models
{
    public class EventModel : MongoDbModel
    {
        [BsonElement]
        public string Title { get; set; }

        [BsonElement]
        public string Description { get; set; }

        [BsonElement]
        public DateTime Start { get; set; }

        [BsonElement]
        public DateTime End { get; set; }

        [BsonElement]
        public bool IsRepeatedByYear { get; set; }

        [BsonElement]
        public List<Location> Locations { get; set; }

        [BsonElement]
        public DateTime AccessAge { get; private set; }

        [BsonElement]
        public int UsersCount { get; set; }

        [BsonElement]
        public Guid CreatorId { get; private set; }

        [BsonElement]
        public List<Guid> Partners { get; private set; }

        [BsonElement]
        public List<Guid> Links { get; set; }

        [BsonElement]
        public EventType Type { get; private set; }

        [BsonElement]
        public EventStatus Status { get; private set; }

        [BsonElement]
        public List<Guid> Data { get; set; }
    }
}
