using MongoDB.Bson.Serialization.Attributes;
using SpaceApp.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceApp.Common.Models
{
    public class DataModel : MongoDbModel
    {
        [BsonElement]
        public DataType Type { get; set; }

        [BsonElement]
        public string Content { get; set; }
    }
}
