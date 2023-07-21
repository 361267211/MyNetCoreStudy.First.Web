using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.Web.FxDto.FxCommonDto;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.FxService.FxFondService;
using NetCoreStudy.First.Web.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.Controllers.FondManger
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FondController : ControllerBase
    {
        private readonly IFondService _fondService;
        private readonly IContactService _contactService;
        private readonly IFondEventService _eventService;
        private readonly IHttpContextAccessor _httpContext;

        public FondController(IFondService fondService, IContactService contactService, IFondEventService eventService, IHttpContextAccessor httpContext)
        {
            _fondService = fondService;
            _contactService = contactService;
            _eventService = eventService;
            _httpContext = httpContext;
        }



        /// <summary>
        /// 根据事件id 查Fonds列表
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<FxFondDto>> GetFondsByEvent(string eventId)
        {
            return await _fondService.GetFontsByEvent(eventId);

        }

        [HttpPost]
        public async Task UploadFile(IFormFile file)
        {
            /*            if (file == null || file.Length == 0)
                            return Content("文件不能为空");*/
            var strDate = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/uploads/{strDate}");
            //判断是否存在保存文件的文件夹,不存在就创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, file.Name);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task UpdateFondsByExcel([FromQuery] string eventId, [FromForm] List<IFormFile> formFiles)
        {
            var file = formFiles[0];
            List<FondExcItem> rows = new List<FondExcItem>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                rows = stream.Query<FondExcItem>().Where(e => !string.IsNullOrEmpty(e.Name)).ToList();
            }
            var dtoEvent = await _eventService.GetEventById(eventId);

            var lstContactDto = rows.Adapt<List<FxContactDto>>();
            //1.根据 contact 姓名 获取 FxContactDto ，之前不存在的会被创建
            List<FxContactDto> lstdtoContact = await _contactService.GetContactsInExcel(lstContactDto);

            //2.组装 FondDto
            List<FxFondDto> lstdtoFond = new List<FxFondDto>();
            var inContactId = _httpContext.HttpContext.User.Claims.FirstOrDefault(e => e.Type == "contactId")?.Value;
            foreach (var itemFond in rows)
            {
                lstdtoFond.Add(new FxFondDto
                {
                    Amount = itemFond.Amount,
                    InContactId = inContactId,
                    ExContactId = lstdtoContact.FirstOrDefault(e => e.Name == itemFond.Name).Id,
                    FxFondEventId = eventId
                });
            }

            //3.更新Fond数据,先删后加
            await  _fondService.UpdateFondInEvent(eventId, lstdtoFond);

            /*return RedirectToAction("Index");*/
        }

        public class FondExcItem
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public int Amount { get; set; }
        }
    }
}
