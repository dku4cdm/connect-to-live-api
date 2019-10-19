using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpaceApp.Common.Enums;
using SpaceApp.Common.Models;
using SpaceApp.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using SpaceApp.Common.Helpers;
using System.Threading.Tasks;

namespace SpaceAppDataAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IRepository<EventModel> _repoEvent;
        private IRepository<UserModel> _repoUser;
        private IOptions<Settings> _config;
        private IOptions<Messages> _sysInfo;
        public UserController(IOptions<Settings> settings, IOptions<Messages> messages)
        {
            _config = settings;
            _sysInfo = messages;
            _repoEvent = new MongoDBRepository<EventModel>(_config.Value.DbConn);
            _repoUser = new MongoDBRepository<UserModel>(_config.Value.DbConn);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetUserData([FromHeader] string id)
        {
            var guid = Guid.Empty;
            if (Guid.TryParse(id, out guid))
            {
                var user = _repoUser.Find(x => x.Id == guid).FirstOrDefault();
                if (user != null)
                {
                    if (user.Status == UserStatus.Offline)
                    {
                        return new JsonResult(_sysInfo.Value.UserInfoAccess);
                    }
                    else
                    {
                        var friendsList = _repoUser.Find(x => user.Friends.Contains(x.Id)).Select(x => 
                        {
                            return new
                            {
                                id = x.Id,
                                name = x.Name,
                                surname = x.Surname,
                                sex = x.Sex,
                                status = x.Status.ToString(),
                                age = x.Age,
                                avatar = x.Avatar
                            };
                        });

                        var eventsList = _repoEvent.Find(x => (user.MyEvents.Contains(x.Id) && x.CreatorId == user.Id) 
                                                            || user.SubscribedEvents.Contains(x.Id) 
                                                            || user.FriendEvents.Contains(x.Id))
                            .Select(x =>
                            {
                                return new
                                {
                                    title = x.Title,
                                    description = x.Description,
                                    start = x.Start,
                                    end = x.End,
                                    status = x.Status.ToString(),
                                    usercount = x.UsersCount
                                };
                            });

                        return new JsonResult(new
                        {
                            id = user.Id.ToString(),
                            name = user.Name,
                            surname = user.Surname,
                            email = user.Email,
                            age = user.Age,
                            status = user.Status,
                            sex = user.Sex,
                            created = user.Created,
                            location = user.Location,
                            friends = friendsList,
                            events = eventsList
                        });
                    }
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.UserNotFound);
                }

            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetGlobalUsersData()
        {
            var users =_repoUser.FindAll().Select(x =>
            {
                return new
                {
                    id = x.Id,
                    name = x.Name,
                    surname = x.Surname,
                    age = x.Age,
                    activity = x.SubscribedEvents.Count,
                    avatar = x.Avatar
                };
            });

            return new JsonResult(users);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RemoveFriend([FromBody] JsonData.FriendData data)
        {
            var guid = Guid.Empty;
            var friendGuid = Guid.Empty;
            if (Guid.TryParse(data?.UserId, out guid) && Guid.TryParse(data?.FriendId, out friendGuid))
            {
                var user = _repoUser.Find(x => x.Id == guid).FirstOrDefault();
                if (user != null && user.Status == UserStatus.Active)
                {
                    if (user.Friends.Contains(friendGuid))
                    {
                        user.Friends.Remove(friendGuid);
                        _repoEvent.Find(x => x.CreatorId == friendGuid).ToList().ForEach(x =>
                        {
                            user.FriendEvents.Remove(x.Id);
                        });
                        _repoUser.Save(user);

                        return new JsonResult(_sysInfo.Value.FriendRemoved);
                    }
                    else
                    {
                        return new JsonResult(_sysInfo.Value.FriendNotAvailable);
                    }
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.UserNotFound);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult AddFriend([FromBody] JsonData.FriendData data)
        {
            var guid = Guid.Empty;
            var friendGuid = Guid.Empty;
            if (Guid.TryParse(data?.UserId, out guid) && Guid.TryParse(data?.FriendId, out friendGuid))
            {
                var user = _repoUser.Find(x => x.Id == guid).FirstOrDefault();
                if (user != null && user.Status == UserStatus.Active)
                {
                    if (user.Friends.Contains(friendGuid))
                    {
                        return new JsonResult(_sysInfo.Value.FriendInList);
                    }
                    else
                    {
                        user.Friends.Add(friendGuid);
                        user.FriendEvents.AddRange(_repoEvent.Find(x => x.CreatorId == friendGuid).Select(x => x.Id));
                        _repoUser.Save(user);

                        return new JsonResult(_sysInfo.Value.FriendAdded);
                    }
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.UserNotFound);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult DeleteUser([FromBody] string id)
        {
            var guid = Guid.Empty;
            if (Guid.TryParse(id, out guid))
            {
                var user = _repoUser.Find(x => x.Id == guid).FirstOrDefault();
                if (user != null && user.Status == UserStatus.Active)
                {
                    _repoUser.Find(x => x.Friends.Contains(user.Id)).ToList().ForEach(x =>
                    {
                        x.Friends.Remove(user.Id);
                        _repoUser.Save(x);
                    });

                    _repoEvent.Find(x => user.SubscribedEvents.Contains(x.Id)).ToList().ForEach(x =>
                    {
                        x.UsersCount--;
                        _repoEvent.Save(x);
                    });

                    _repoUser.Remove(user);
                    return new JsonResult(_sysInfo.Value.UserDeleted);
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.UserNotFound);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateUser([FromBody] JsonData.User userData)
        {
            if (userData != null)
            {
                if (_repoUser.Find(x => x.Email == userData.Email).FirstOrDefault() == null)
                {
                    var user = new UserModel(userData.Name, userData.Surname, userData.Age, userData.Avatar, userData.Email, userData.UserType, userData.Sex, userData.Password.EncodePass());
                    _repoUser.Save(user);

                    return new JsonResult(_sysInfo.Value.UserCreated);
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.SameEmail);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Login([FromBody] JsonData.LoginData data)
        {
            if (data != null)
            {
                var user = _repoUser.Find(x => x.Email == data.Email && x.Password == data.Password.EncodePass()).FirstOrDefault();

                if (user != null)
                {
                    user.Status = UserStatus.Active;
                    _repoUser.Save(user);
                    return new JsonResult(new
                    {
                        id = user.Id.ToString(),
                        name = user.Name,
                        surname = user.Surname,
                        email = user.Email,
                        age = user.Age,
                        status = user.Status,
                        sex = user.Sex,
                        avatar = user.Avatar
                    });
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.LoginFail);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Logout([FromBody] string id)
        {
            var guid = Guid.Empty;
            if (id != null && Guid.TryParse(id, out guid))
            {
                var user = _repoUser.Find(x => x.Id == guid).FirstOrDefault();

                if (user != null)
                {
                    user.Status = UserStatus.Offline;
                    _repoUser.Save(user);
                    return new JsonResult(_sysInfo.Value.Logout);
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.LogoutError);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult SubscribeToEvent([FromBody] JsonData.EventSubscribeRequest request)
        {
            var guid = Guid.Empty;
            var eventGuid = Guid.Empty;
            if (Guid.TryParse(request?.UserId, out guid) && Guid.TryParse(request?.EventId, out eventGuid))
            {
                var user = _repoUser.Find(x => x.Id == guid).FirstOrDefault();
                var eventItem = _repoEvent.Find(x => x.Id == eventGuid).FirstOrDefault();

                if (user != null && user.Status == UserStatus.Active)
                {
                    if (eventItem != null)
                    {
                        eventItem.UsersCount++;
                        _repoEvent.Save(eventItem);
                        user.SubscribedEvents.Add(eventItem.Id);
                        _repoUser.Save(user);

                        return new JsonResult(_sysInfo.Value.EventSubscribe);
                    }
                    else
                    {
                        return new JsonResult(_sysInfo.Value.EventNotFound);
                    }
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.UserNotFound);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UnsubscribeFromEvent([FromBody] JsonData.EventSubscribeRequest request)
        {
            var guid = Guid.Empty;
            var eventGuid = Guid.Empty;
            if (Guid.TryParse(request?.UserId, out guid) && Guid.TryParse(request?.EventId, out eventGuid))
            {
                var user = _repoUser.Find(x => x.Id == guid).FirstOrDefault();
                var eventItem = _repoEvent.Find(x => x.Id == eventGuid).FirstOrDefault();

                if (user != null && user.Status == UserStatus.Active)
                {
                    if (eventItem != null && user.SubscribedEvents.Contains(eventItem.Id))
                    {
                        eventItem.UsersCount--;
                        _repoEvent.Save(eventItem);
                        user.SubscribedEvents.Remove(eventItem.Id);
                        _repoUser.Save(user);

                        return new JsonResult(_sysInfo.Value.EventUnsubscribe);
                    }
                    else
                    {
                        return new JsonResult(_sysInfo.Value.EventNotFound);
                    }
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.UserNotFound);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EditUserData([FromBody] JsonData.User user)
        {
            // TODO
            return new JsonResult(new { });
        }
    }
}
