using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Web.FxDto.FxFond;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public class FondRepository : IFondRepository
    {
        private readonly FondDbContext _fondDb;

        public FondRepository(FondDbContext fondDb)
        {
            _fondDb = fondDb;
        }

        public async Task<List<FxFond>> GetFondsByEvent(string eventId)
        {
            var Ft = await _fondDb.FondEvents.FirstOrDefaultAsync(e => e.Id == eventId);
            return Ft.Fonds;
        }
    }
}
