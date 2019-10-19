using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SpaceApp.Common.Models;
using SpaceApp.Common.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceAppDataAPI.Controllers
{
    [Route("api/[controller]")]
    [EnableCors]
    [ApiController]
    public class EventController : ControllerBase
    {
        private IRepository<DataModel> _repoData;
        private IRepository<EventModel> _repoEvent;
        private IRepository<UserModel> _repoUser;
        private IOptions<Settings> _config;
        private IOptions<Messages> _sysInfo;
        public EventController(IOptions<Settings> settings, IOptions<Messages> messages)
        {
            _config = settings;
            _sysInfo = messages;
            _repoData = new MongoDBRepository<DataModel>(_config.Value.DbConn);
            _repoEvent = new MongoDBRepository<EventModel>(_config.Value.DbConn);
            _repoUser = new MongoDBRepository<UserModel>(_config.Value.DbConn);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetEventData([FromHeader] string id)
        {
            var guid = Guid.Empty;
            if (Guid.TryParse(id, out guid))
            {
                var selectedEvent = _repoEvent.Find(x => x.Id == guid).FirstOrDefault();
                if (selectedEvent != null)
                {
                    var relatedData = _repoData.Find(x => selectedEvent.Data.Contains(x.Id)) ?? new List<DataModel>();

                    var info = relatedData.Select(s =>
                    {
                        return new
                        {
                            id = s.Id,
                            type = s.Type.ToString(),
                            content = s.Content,
                            uploaded = s.Created
                        };
                    });

                    var user = _repoUser.Find(x => x.Id == selectedEvent.CreatorId).FirstOrDefault();

                    return new JsonResult(new
                    {
                        id = selectedEvent.Id,
                        description = selectedEvent.Description,
                        start = selectedEvent.Start,
                        end = selectedEvent.End,
                        type = selectedEvent.Type,
                        status = selectedEvent.Status,
                        popularity = selectedEvent.UsersCount,
                        data = info,
                        author = $"{user?.Name} {user?.Surname}"
                    });


                }
                else
                {
                    return new JsonResult(_sysInfo.Value.EventNotFound);
                }
            }
            else
            {
                return new JsonResult(new List<DataModel>());
            }
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult CreateEvent([FromBody] JsonData.Event eventData)
        {
            // TODO
            return new JsonResult(new { });
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult RemoveEvent([FromBody] JsonData.Event eventData)
        {
            // TODO
            return new JsonResult(new { });
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetGlobalEventData()
        {
            // TODO
            return new JsonResult(new { });
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EditEvent([FromBody] JsonData.Event eventData)
        {
            // TODO
            return new JsonResult(new { });
        }
    }
}
