using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPanel.Infrastructure.Persistence.Data;

namespace AdminPanel.Application.Interfaces
{
    public interface ICharityCategoryRepository
    {
        Task<List<CharityCategory>> GetByCharityIdAsync(int charityId);
        Task RemoveRangeAsync(List<CharityCategory> items);
        Task AddAsync(CharityCategory entity);
    }
}
