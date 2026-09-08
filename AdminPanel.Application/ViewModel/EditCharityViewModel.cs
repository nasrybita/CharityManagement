using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace AdminPanel.Application.ViewModel
{
    public class EditCharityViewModel
    {
        // اطلاعات پایه
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? Telephone { get; set; }
        public string? Address { get; set; }
        public string? ManagerName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }

        // لیست‌ها برای Binding
        public List<int> CategoryIds { get; set; } = new();
        public List<CharitySocialMediaViewModel> SocialMedias { get; set; } = new();

        // فایل‌ها برای آپلود
        public IFormFile? LogoFile { get; set; }
        public IFormFile? BannerFile { get; set; }
    }

    public class CharitySocialMediaViewModel
    {
        public int? Id { get; set; }
        public int? SocialId { get; set; }
        public string? SocialName { get; set; }
        public string? Value { get; set; }
        public string? IconUrl { get; set; }
    }
}

