using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceAppDataAPI
{
    public class Messages
    {
        public string TypeNull { get; set; }
        public string UserCreated { get; set; }
        public string UserNotFound { get; set; }
        public string UserDeleted { get; set; }
        public string FriendInList { get; set; }
        public string FriendAdded { get; set; }
        public string FriendRemoved { get; set; }
        public string FriendNotAvailable { get; set; }
        public string EventNotFound { get; set; }
        public string EventSubscribe { get; set; }
        public string EventUnsubscribe { get; set; }
        public string UserInfoAccess { get; set; }
        public string LoginFail { get; set; }
        public string SameEmail { get; set; }
        public string Logout { get; set; }
        public string LogoutError { get; set; }
        public string UploadFinished { get; set; }
        public string UploadFailed { get; set; }
        public string NotEnoughAccess { get; set; }
        public string ContentNotFound { get; set; }
        public string ContentWasDeleted { get; set; }
    }
}
