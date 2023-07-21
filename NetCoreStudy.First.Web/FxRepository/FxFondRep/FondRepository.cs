using Mapster;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Web.FxDto.FxFond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZSpitz.Util;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public class FondRepository : IFondRepository
    {
        private readonly FondDbContext _fondDb;

        public FondRepository(FondDbContext fondDb)
        {
            _fondDb = fondDb;
        }

        public async Task AddFonds(List<FxFondDto> lstdtoFond)
        {
            var lstetyFond = lstdtoFond.Adapt<List<FxFond>>();
            _fondDb.Fonds.AddRange(lstetyFond);

            await _fondDb.SaveChangesAsync();
        }

        public async Task DeleteFondInEvent(string eventId)
        {
            _fondDb.Fonds.RemoveRange(_fondDb.Fonds.Where(e=>e.FxFondEventId== eventId));
            await _fondDb.SaveChangesAsync();
        }

        public async Task<List<FxFond>> GetFondsByEvent(string eventId)
        {
            var eve = await _fondDb.FondEvents.Include(d => d.Fonds).FirstOrDefaultAsync(e => e.Id == eventId);
            var Ft = await _fondDb.Fonds.Where(e => e.FxFondEventId == eventId).ToListAsync();
            return Ft;
        }
    }
}
