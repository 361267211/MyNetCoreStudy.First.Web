using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Web.FxDto.FxFond;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public class FondEventRepository : IFondEventRepository
    {
        private readonly FondDbContext _eventDb;

        public FondEventRepository(FondDbContext eventDb)
        {
            _eventDb = eventDb;
        }

        public async Task<List<FxFondEvent>> GetEventsByInitiator(string contactId)
        {
            return  await _eventDb.FondEvents.Where(e => e.EventInitiator == contactId).ToListAsync() ;
        }
    }
}
