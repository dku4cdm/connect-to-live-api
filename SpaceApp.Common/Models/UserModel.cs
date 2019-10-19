using MongoDB.Bson.Serialization.Attributes;
using SpaceApp.Common.Enums;
using System;
using System.Collections.Generic;

namespace SpaceApp.Common.Models
{
    public class UserModel : MongoDbModel
    {
        [BsonElement]
        public string Name { get; private set; }
        [BsonElement]
        public string Surname { get; private set; }
        public string Avatar { get; private set; }
        [BsonElement]
        public string Password { get; private set; }
        [BsonElement]
        public string Email { get; private set; }
        [BsonElement]
        public UserType UserType { get; private set; }
        [BsonElement]
        public DateTime Age { get; private set; }
        [BsonElement]
        public Sex Sex { get; private set; }
        [BsonElement]
        public UserStatus Status { get; set; }
        [BsonElement]
        public List<Guid> Friends { get; set; }
        [BsonElement]
        public List<Guid> SubscribedEvents { get; set; }
        [BsonElement]
        public List<Guid> MyEvents { get; set; }
        [BsonElement]
        public List<Guid> FriendEvents { get; set; }
        [BsonElement]
        public Location Location { get; set; }
        public UserModel() { }

        public UserModel(string name, string surname, DateTime age, string avatar, string email, UserType userType, Sex sex, string password)
        {
            Name = name;
            Surname = surname;
            Age = age;
            Avatar = avatar;
            Email = email;
            Password = password;
            UserType = userType;
            Sex = Sex;
            FriendEvents = new List<Guid>();
            Friends = new List<Guid>();
            Location = new Location().SetLocation(0, 0);
            Status = UserStatus.Offline;
        }

        public UserModel(string name, string surname, string email, UserType type, string password)
        {
            Name = name;
            Surname = surname;
            UserType = type;
            Email = email;
            Password = password;
            FriendEvents = new List<Guid>();
            Friends = new List<Guid>();
            Location = new Location().SetLocation(0, 0);
            Status = UserStatus.Offline;
        }
    }
}
