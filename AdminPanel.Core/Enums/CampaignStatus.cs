using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.Enums
{
    public enum CampaignStatus
    {
        [Display(Name = "ایجاد شده")]
        Created = 1,

        [Display(Name = "تایید شده")]
        Approved = 2,

        [Display(Name = "رد شده")]
        Rejected = 3,

        [Display(Name = "تعلیق شده")]
        Suspended = 4,

        [Display(Name = "فعال‌سازی مجدد")]
        Reactivated = 5,

        [Display(Name = "تأمین مالی موفق")]
        Funded = 6,

        [Display(Name = "تأمین مالی ناموفق")]
        NotFunded = 7,

        [Display(Name = "تعلیق توسط سیستم")]
        SuspendedBySystem = 8,

        [Display(Name = "تعلیق توسط خیریه")]
        SuspendedByCharity = 9,

        [Display(Name = "رد شده توسط سیستم")]
        RejectedBySystem = 10,

        [Display(Name = "رد شده توسط خیریه")]
        RejectedByCharity = 11

    }
}
