using AdminPanel.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Interfaces
{
    public interface IFileStorageService
    {
        Task<StoredFile> SaveLogoAsync(IFormFile file);
        Task<StoredFile> SaveBannerAsync(IFormFile file);
        Task DeleteAsync(int fileId);

    }
}
