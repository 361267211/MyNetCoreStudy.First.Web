using DotNetCore.CAP;
using FxCode.FxDatabaseAccessor;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxService.FxFondService;
using NetCoreStudy.First.Web.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers.FondManger
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FondEventController
    {
        private readonly IFondService _fondService;
        private readonly IContactService _contactService;
        private readonly IFondEventService _eventService;
        private readonly IHttpContextAccessor _http;
        private readonly FondDbContext _dbContext;


        private readonly ICapPublisher _capBus;



        public FondEventController(IFondService fondService, IContactService contactService, IFondEventService eventService, IHttpContextAccessor httpContet, ICapPublisher capBus, FondDbContext dbContext)
        {
            _fondService = fondService;
            _contactService = contactService;
            _eventService = eventService;
            _http = httpContet;
            _capBus = capBus;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 根据事件主导者查询事件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<FxFondEventDto>> GetEventsByInitiator(string contactId)
        {
            return await _fondService.GetEventsByInitiator(contactId);
        }

        /// <summary>
        /// 根据符合条件查询事件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<List<FxFondEventDto>> GetEventsByCondition([FromBody] EventSearchConditionRequest condition)
        {
            return await _eventService.GetEventsByCondition(condition);
        }

        /// <summary>
        /// 通过id获取事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<FxFondEventDto> GetEventById(string eventId)
        {
            return await _eventService.GetEventById(eventId);
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="eventDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<bool> UpdateEvent(FxFondEventDto eventDto)
        {
            var contactId = _http.HttpContext.User.Claims.FirstOrDefault(e => e.Type == "contactId")?.Value;
            if (contactId != null)
            {
                eventDto.EventInitiator = contactId;
            }
            return await _eventService.UpdateEvent(eventDto);

        }

        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="eventDto"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<bool> DeleteEvent(string eventId)
        {

            return await _eventService.DeleteEvent(eventId);

        }

        /// <summary>
        /// 发布案例
        /// </summary>
        /// <param name="eventDto"></param>
        /// <returns></returns>
        //[Authorize]
        [UnitOfWork]
        [HttpGet]
        public async Task<bool> SendMessage(string eventId)
        {

            try
            {
               // using var tran = _dbContext.Database.BeginTransaction(_capBus);
                await _eventService.UpdateEvent(new FxFondEventDto() { Name="测试一下cap" });

                var headers = new Dictionary<string, string>()
                {
                    ["felix.header.first"] = "first",
                    ["felix.header.second"] = "second"
                };
                await _capBus.PublishAsync(name: "fxFondEventDeleteLog",
                    contentObj: new FxFondDto() { FxFondEventId = eventId },
                    headers: headers
                    );
            }
            catch (Exception)
            {

                throw new Exception();
            }


            return true;// await _eventService.DeleteEvent(eventId);

        }


        [NonAction]
        [CapSubscribe("fxFondEventDeleteLog")]
        public async Task<string> ReceiveMessage(FxFondDto fondDto, [FromCap] CapHeader header)
        {

            await Task.Delay(1 * 30 * 1000);
            Console.WriteLine("message eventId is:" + fondDto.FxFondEventId);
            Console.WriteLine("message firset header :" + header["felix.header.first"]);
            Console.WriteLine("message second header :" + header["felix.header.second"]);

            if (fondDto.FxFondEventId == "9527")
            {
                throw new Exception();

            }
            return fondDto.FxFondEventId;
        }

    }
}
