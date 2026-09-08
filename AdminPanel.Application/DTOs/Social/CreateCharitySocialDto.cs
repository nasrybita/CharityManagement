using AdminPanel.Application.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.Social
{
    //CreateCharitySocialDto.cs is created so that we can use it for creating socials inside "Create Charity Page"
    // NOTE: CreateSocialRequestDto.cs must remain separate so that we can also use it for social management only (we don't have separate social management section yet)
    public class CreateCharitySocialDto
    {
        //ID of Social Media previously built by system admin
        public int SocialId { get; set; }

        //Link of Social Media
        public string Value { get; set; } = null!;

    }
}
