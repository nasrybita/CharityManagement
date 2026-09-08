
﻿#region old code
//using AdminPanel.Application.Interfaces;
//using AdminPanel.Core.Entities;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AdminPanel.Infrastructure.Services
//{
//    public class FileStorageService : IFileStorageService
//    {
//        private readonly IWebHostEnvironment _env;
//        private readonly AdminPanelDbContext _context;

//        public FileStorageService(IWebHostEnvironment env, AdminPanelDbContext context)
//        {
//            _env = env;
//            _context = context;
//        }




//        public async Task<StoredFile> SaveLogoAsync(IFormFile file)
//        {
//            return await SaveFile(file, "uploads/charities/logos");
//        }



//        public async Task<StoredFile> SaveBannerAsync(IFormFile file)
//        {
//            return await SaveFile(file, "uploads/charities/banners");
//        }



//        private async Task<StoredFile> SaveFile(IFormFile file, string folder)
//        {
//            // 1. Ensure the file is not null or empty
//            if (file == null || file.Length == 0)
//                throw new ArgumentException("فایل خالی است");


//            // 2. Define the list of allowed file extensions
//            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };


//            // 3.Extract file extension from file name
//            var extension = Path.GetExtension(file.FileName).ToLower();


//            // 4. Reject files with unsupported extensions
//            if (!allowedExtensions.Contains(extension))
//                throw new ArgumentException("فایل با این پسوند مجاز نمیباشد");


//            // 5. Reject files larger than 5 MB
//            if (file.Length > 5 * 1024 * 1024) // 5MB
//                throw new ArgumentException("سایز فایل بیشتر از حد مجاز است(حداکثر حجم فایل: 5 مگ)");


//            // 6. Create a unique file name
//            var fileName = $"{Guid.NewGuid()}{extension}";


//            // 7. Combine wwwroot path with specified folder path
//            var folderPath = Path.Combine(_env.WebRootPath, folder);


//            // 8. If that folder doesn't exist now, create it
//            if (!Directory.Exists(folderPath))
//            {
//                Directory.CreateDirectory(folderPath);
//            }



//            // 9. Create full file path (the path where the file is saved in)
//            var fullPath = Path.Combine(folderPath, fileName);


//            // 10. Perform main save operation
//            using (var stream = new FileStream(fullPath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }


//            // 11. Create an object of StoredFile to save file data in database
//            var storedFile = new StoredFile
//            {
//                FileName = fileName,
//                FilePath = $"/{folder}/{fileName}",
//                FileType = file.ContentType,
//                CreatedAt = DateTime.UtcNow,
//                ModifiedAt = DateTime.UtcNow,
//                IsDeleted = false
//            };


//            // 12. Add record to StoredFiles table in database
//            _context.StoredFiles.Add(storedFile);


//            // 13. Changes save into database
//            await _context.SaveChangesAsync();


//            // 14. Created record is returned(so that service or controller can send it to client)
//            return storedFile;
//        }

//        public async Task DeleteAsync(int fileId)
//        {
//            // 1. Retrieve the file record from the database by its ID
//            var file = await _context.StoredFiles.FindAsync(fileId);


//            // 2. Throw an exception if the file record does not exist
//            if (file == null)
//            {
//                throw new Exception("فایل یافت نشد");
//            }


//            // 3. Build the physical file path from the stored relative path
//            var physicalPath = Path.Combine(_env.WebRootPath, file.FilePath.TrimStart('/'));


//            // 4. Delete the physical file from the server if it exists
//            if (File.Exists(physicalPath))
//            {
//                File.Delete(physicalPath);
//            }


//            // 5. Mark the file as deleted in the database
//            file.IsDeleted = true;


//            // 6. Update the modification timestamp
//            file.ModifiedAt = DateTime.UtcNow;


//            // 7. Save the changes to the database
//            await _context.SaveChangesAsync();
//        }




//    }
//}
#endregion



using AdminPanel.Application.Interfaces;
using AdminPanel.Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly AdminPanelDbContext _context;

        public FileStorageService(IWebHostEnvironment env, AdminPanelDbContext context)
        {
            _env = env;
            _context = context;
        }

        public async Task<StoredFile> SaveLogoAsync(IFormFile file)
        {
            return await SaveFile(file, "uploads/charities/logos");
        }

        public async Task<StoredFile> SaveBannerAsync(IFormFile file)
        {
            return await SaveFile(file, "uploads/charities/banners");
        }

        //private async Task<StoredFile> SaveFile(IFormFile file, string folder)

        //{



        //    if (file == null || file.Length == 0)
        //        throw new ArgumentException("فایل خالی است");

        //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        //    var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

        //    if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
        //        throw new ArgumentException("فایل با این پسوند مجاز نمیباشد");

        //    if (file.Length > 5 * 1024 * 1024)
        //        throw new ArgumentException("سایز فایل بیشتر از حد مجاز است(حداکثر حجم فایل: 5 مگ)");

        //    var fileName = $"{Guid.NewGuid()}{extension}";
        //    var webRootPath = GetWebRootPath();
        //    var folderPath = Path.Combine(webRootPath, folder);

        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }

        //    var fullPath = Path.Combine(folderPath, fileName);


        //    ////test
        //    //// در متد SaveFile، بعد از خط var fullPath = ...
        //    //Console.WriteLine($"DEBUG PATH: {fullPath}");
        //    ////test

        //    using (var stream = new FileStream(fullPath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    var storedFile = new StoredFile
        //    {
        //        FileName = fileName,
        //        FilePath = $"/{folder.Replace("\\", "/")}/{fileName}",
        //        FileType = file.ContentType,
        //        CreatedAt = DateTime.UtcNow,
        //        ModifiedAt = DateTime.UtcNow,
        //        IsDeleted = false
        //    };

        //    _context.StoredFiles.Add(storedFile);
        //    await _context.SaveChangesAsync();

        //    return storedFile;
        //}





        //new code

        private async Task<StoredFile> SaveFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("فایل خالی است");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
                throw new ArgumentException("فایل با این پسوند مجاز نمیباشد");

            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("سایز فایل بیشتر از حد مجاز است(حداکثر حجم فایل: 5 مگ)");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var webRootPath = GetWebRootPath();
            var normalizedFolder = folder.Replace("/", Path.DirectorySeparatorChar.ToString());
            var folderPath = Path.Combine(webRootPath, normalizedFolder);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var storedFile = new StoredFile
            {
                FileName = fileName,
                FilePath = $"/{folder.Replace("\\", "/")}/{fileName}",
                FileType = file.ContentType,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.StoredFiles.Add(storedFile);
            await _context.SaveChangesAsync();

            return storedFile;
        }
//new code

        public async Task DeleteAsync(int fileId)
        {
            var file = await _context.StoredFiles.FindAsync(fileId);

            if (file == null)
                throw new Exception("فایل یافت نشد");

            var webRootPath = GetWebRootPath();
            var relativePath = file.FilePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var physicalPath = Path.Combine(webRootPath, relativePath);

            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            file.IsDeleted = true;
            file.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private string GetWebRootPath()
        {
            var webRootPath = _env.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
            }

            if (!Directory.Exists(webRootPath))
            {
                Directory.CreateDirectory(webRootPath);
            }

            return webRootPath;
        }
    }
}
