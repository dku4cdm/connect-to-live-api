using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpaceApp.Common.Enums;
using SpaceApp.Common.Models;
using SpaceApp.Common.Repository;

namespace SpaceAppDataAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class DataController : ControllerBase
    {
        private IRepository<DataModel> _repoData;
        private IRepository<EventModel> _repoEvent;
        private IRepository<UserModel> _repoUser;
        private IOptions<Settings> _config;
        private IOptions<Messages> _sysInfo;
        public DataController(IOptions<Settings> settings, IOptions<Messages> messages)
        {
            _config = settings;
            _sysInfo = messages;
            _repoData = new MongoDBRepository<DataModel>(_config.Value.DbConn);
            _repoEvent = new MongoDBRepository<EventModel>(_config.Value.DbConn);
            _repoUser = new MongoDBRepository<UserModel>(_config.Value.DbConn);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateData([FromBody] JsonData.AddDataRequest data)
        {
            var guid = Guid.Empty;
            if (data != null && Guid.TryParse(data.EventId, out guid))
            {
                var selectedEvent = _repoEvent.Find(x => x.Id == guid).FirstOrDefault();
                if (selectedEvent != null)
                {
                    var dataToSave = new DataModel()
                    {
                        Type = data.Type,
                        Content = data.Content
                    };
                    _repoData.Save(dataToSave);

                    var dataId = _repoData.Find(x => x.Type == dataToSave.Type && x.Content == dataToSave.Content).LastOrDefault()?.Id;

                    if (dataId.HasValue)
                    {
                        selectedEvent.Data.Add(dataId.Value);
                        _repoEvent.Save(selectedEvent);
                    }
                    return new JsonResult(_sysInfo.Value.UploadFinished);
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.EventNotFound);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RemoveData([FromBody] JsonData.RemoveDataRequest data)
        {
            var guid = Guid.Empty;
            var dataGuid = Guid.Empty;
            var eventGuid = Guid.Empty;
            if (data != null && Guid.TryParse(data?.UserId, out guid) && Guid.TryParse(data?.DataId, out dataGuid) && Guid.TryParse(data?.EventId, out eventGuid))
            {
                var selectedEvent = _repoEvent.Find(x => x.Id == eventGuid).FirstOrDefault();
                var selectedData = _repoData.Find(x => x.Id == dataGuid).FirstOrDefault();
                var user =  _repoUser.Find(x => x.Id == guid).FirstOrDefault();

                if (user != null && (user.UserType == UserType.Simple || user.UserType == UserType.Moderator || user.UserType == UserType.Admin))
                {
                    if (selectedEvent != null)
                    {
                        if (selectedData != null)
                        {
                            selectedEvent.Data.Remove(selectedData.Id);
                            _repoEvent.Save(selectedEvent);
                            _repoData.Remove(selectedData);

                            return new JsonResult(_sysInfo.Value.ContentWasDeleted);
                        }
                        else
                        {
                            return new JsonResult(_sysInfo.Value.ContentNotFound);
                        }
                    }
                    else
                    {
                        return new JsonResult(_sysInfo.Value.EventNotFound);
                    }
                }
                else
                {
                    return new JsonResult(_sysInfo.Value.NotEnoughAccess);
                }
            }
            else
            {
                return new JsonResult(_sysInfo.Value.TypeNull);
            }
        }
    }
}
