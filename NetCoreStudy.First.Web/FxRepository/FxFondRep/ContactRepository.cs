using Mapster;
using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore;
using NetCoreStudy.First.Web.FxDto.FxCommonDto;
using NetCoreStudy.First.Web.FxDto.FxFond;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.FxRepository.FxFondRep
{
    public class ContactRepository : IContactRepository
    {
        private readonly FondDbContext _fondDb;

        public ContactRepository(FondDbContext fondDb)
        {
            _fondDb = fondDb;
        }

        public async Task<List<FxContact>> AddContactBatch(List<FxContactDto> lstContact)
        {
            var lstContactEnt = lstContact.Adapt<List<FxContact>>();
            lstContactEnt.ForEach(e => e.Id = Guid.NewGuid().ToString());
            _fondDb.FondContacts.AddRange(lstContactEnt);
            await _fondDb.SaveChangesAsync();
            return lstContactEnt;
        }

        public async Task<List<FxContact>> GetContactsByName(string nameCondition)
        {
            return await _fondDb.FondContacts.Where(e => e.Name.StartsWith(nameCondition)).ToListAsync();
        }

        public async Task<List<FxContactDto>> GetContactsByNameList(List<string> nameList)
        {
            return await _fondDb.FondContacts.ProjectToType<FxContactDto>().Where(e => nameList.Contains(e.Name)).ToListAsync();
        }

        public async Task<List<OptionDto>> GetContactsOptions()
        {
            List<OptionDto> options = await _fondDb.FondContacts.Select(e => new OptionDto(e.Name, e.Id)).ToListAsync();
            return options;
        }

        public async Task<List<FxContact>> GetFxContactsByFonds(List<string> fondIds)
        {
            var lstCons = await _fondDb.FondContacts.Where(e => fondIds.Contains(e.Id)).ToListAsync();
            return lstCons;
        }
    }
}
