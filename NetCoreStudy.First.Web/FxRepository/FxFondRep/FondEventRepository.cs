using Mapster;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Web.FxDto.FxFond;
using NetCoreStudy.First.Web.Request;
using System;
using System.Collections.Generic;
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
             
            if (eventDto.Id.IsNullOrEmpty())
            {
                eventDto.Id = Guid.NewGuid().ToString();
                await _eventDb.FondEvents.AddAsync(eventDto.Adapt<FxFondEvent>());
            }
            else
            {
                var ent = _eventDb.FondEvents.Update(eventDto.Adapt<FxFondEvent>());
            }
            await _eventDb.SaveChangesAsync();
            return true;
        }
    }
}
