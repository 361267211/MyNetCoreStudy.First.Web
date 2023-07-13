﻿using Microsoft.EntityFrameworkCore;
using NetCoreStudy.First.Domain.Entity.Fond;
using NetCoreStudy.First.EFCore;
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

        public async Task<List<FxContact>> GetContactsByName(string nameCondition)
        {
            return await _fondDb.FondContacts.Where(e => e.Name.StartsWith(nameCondition)).ToListAsync();
        }

        public async Task<List<FxContact>> GetFxContactsByFonds(List<string> fondIds)
        {
            var lstCons = await _fondDb.FondContacts.Where(e => fondIds.Contains(e.Id)).ToListAsync();
            return lstCons;
        }
    }
}
