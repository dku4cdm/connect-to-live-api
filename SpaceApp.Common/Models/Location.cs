using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceApp.Common.Models
{
    public class Location
    {
        [BsonElement]
        public double Longitude { get; private set; }

        [BsonElement]
        public double Latitude { get; private set; }

        public Location SetLocation(double longitude, double latitude)
        {
            Latitude = latitude;
            Longitude = longitude;

            return this;
        }
    }
}
