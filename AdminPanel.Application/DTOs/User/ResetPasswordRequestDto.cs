using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.DTOs.User
{
    
        public class ResetPasswordRequestDto
    {
        public string? CurrentPassword { get; set; }


        [Required(ErrorMessage = "رمز عبور جدید الزامی است")]
        [StringLength(
            100,
            MinimumLength = 6,
            ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد"
        )]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "تکرار رمز عبور الزامی است")]
        public string ConfirmPassword { get; set; }


    }

    }

