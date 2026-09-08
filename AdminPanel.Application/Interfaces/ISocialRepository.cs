using AdminPanel.Infrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace AdminPanel.Application.Interfaces
{
    public interface ISocialRepository
    {
        Task<bool> IsNameExistAsync(string name);

        Task AddAsync(Social social);
        Task<Social?> GetByIdAsync(int id);

        Task SaveChangesAsync();
        Task UpdateAsync(Social social);

        Task<List<Social>> GetAllAsync();
        Task DeleteAsync(Social social);
    }
}
