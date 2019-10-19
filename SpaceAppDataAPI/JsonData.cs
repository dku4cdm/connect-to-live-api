using SpaceApp.Common.Enums;
using SpaceApp.Common.Models;
using System;
using System.Collections.Generic;

namespace SpaceAppDataAPI
{
    public class JsonData
    {
        public class AddDataRequest
        {
            public string EventId { get; set; }
            public DataType Type { get; set; }
            public string Content { get; set; }
        }

        public class Event
        {
            public string Title { get; set; }
            public string Id { get; set; }
            public string Description { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public bool IsRepeatedByYear { get; set; }
            public List<Location> Locations { get; set; }
            public DateTime AccessAge { get; set; }
            public EventType Type { get; set; }
        }

        public class User
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Avatar { get; set; }
            public string Password { get; set; }
            public string Id { get; set; }
            public string Surname { get; set; }
            public UserType UserType { get; set; }
            public UserStatus Status { get; set; }
            public DateTime Age { get; set; }
            public Sex Sex { get; set; }
            public Location Location { get; set; }
        }

        public class LoginData
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class FriendData
        {
            public string UserId { get; set; }
            public string FriendId { get; set; }
        }

        public class EventSubscribeRequest
        {
            public string UserId { get; set; }
            public string EventId { get; set; }
        }

        public class RemoveDataRequest
        {
            public string UserId { get; set; }
            public string EventId { get; set; }
            public string DataId { get; set; }
        }
    }
}
