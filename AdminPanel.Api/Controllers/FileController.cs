using AdminPanel.Application.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace AdminPanel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public FileController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            var storedFile = await _fileStorageService.SaveLogoAsync(file);
            return Ok(storedFile);
        }

        [HttpPost("upload-banner")]
        public async Task<IActionResult> UploadBanner(IFormFile file)
        {
            var storedFile = await _fileStorageService.SaveBannerAsync(file);
            return Ok(storedFile);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            await _fileStorageService.DeleteAsync(id);
            return NoContent();
        }
    }
}