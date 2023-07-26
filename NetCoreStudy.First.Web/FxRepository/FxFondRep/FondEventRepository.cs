using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZSpitz.Util;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public class FondEventRepository : IFondEventRepository
    {
        private readonly FondDbContext _eventDb;

        public FondEventRepository(FondDbContext eventDb)
        {
            _eventDb = eventDb;
        }

        public async Task<bool> DeleteEvent(string eventId)
        {
            var eventEntity = await _eventDb.FondEvents.FindAsync(eventId);
            _eventDb.FondEvents.Remove(eventEntity);
            await _eventDb.SaveChangesAsync();
            return true;
        }

        public async Task<FxFondEvent> GetEventById(string eventId)
        {
            return await _eventDb.FondEvents.FindAsync(eventId);
        }

        public async Task<List<FxFondEvent>> GetEventsByCondition(EventSearchConditionRequest condition)
        {
            var query = _eventDb.FondEvents.Where(e => true);

            if (condition.StartDate.HasValue)
            {
                query = query.Where(e => e.StartDate > condition.StartDate);
            }
            if (condition.EndDate.HasValue)
            {
                query = query.Where(e => e.StartDate < condition.StartDate);
            }
            if (!string.IsNullOrEmpty(condition.NamePrefix))
            {
                query = query.Where(e => e.Name.StartsWith(condition.NamePrefix));
            }
            if (condition.Initiators.Count > 0)
            {
                query = query.Where(e => condition.Initiators.Contains(e.EventInitiator));

            }

            var lstEvent = await query.ToListAsync();
            return lstEvent;
        }

        public async Task<List<FxFondEvent>> GetEventsByInitiator(string contactId)
        {
            return await _eventDb.FondEvents.Where(e => e.EventInitiator == contactId).ToListAsync();
        }

        public async Task<bool> UpdateEvent(FxFondEventDto eventDto)
        {
            var etyEvent = eventDto.Adapt<FxFondEvent>();

            if (eventDto.Id.IsNullOrEmpty())
            {
                await _eventDb.FondEvents.AddAsync(etyEvent);
            }
            else
            {
                var res = _eventDb.FondEvents.Update(etyEvent);
            }
          //  await _eventDb.SaveChangesAsync();
            return true;
        }

        [HttpPost]
        public async Task UploadExcel(IFormFile file)
        {
            var rows = MiniExcel.Query<UserAccount>("");
        }
        public class UserAccount
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public DateTime BoD { get; set; }
            public int Age { get; set; }
            public bool VIP { get; set; }
            public decimal Points { get; set; }
        }
    }
}
