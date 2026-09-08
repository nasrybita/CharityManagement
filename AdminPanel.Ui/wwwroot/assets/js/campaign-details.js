'use strict';

$(async function () {
    const token = $('meta[name="access-token"]').attr('content');
    const userRole = $('meta[name="user-role"]').attr('content');
    const campaignId = $('#campaign-id').val();
    const apiUrl = `${apiBaseUrl}/api/campaign/${campaignId}`;

    const statusMap = {
        1: { text: 'ایجاد شده', class: 'bg-label-secondary' },
        2: { text: 'تایید شده', class: 'bg-label-success' },
        3: { text: 'رد شده', class: 'bg-label-danger' },
        4: { text: 'تعلیق شده', class: 'bg-label-warning' },
        5: { text: 'فعال‌سازی مجدد', class: 'bg-label-info' },
        6: { text: 'تأمین مالی موفق', class: 'bg-label-primary' },
        7: { text: 'تأمین مالی ناموفق', class: 'bg-label-dark' },
        8: { text: 'تعلیق توسط سیستم', class: 'bg-label-danger' },
        9: { text: 'تعلیق توسط خیریه', class: 'bg-label-warning' },
        10: { text: 'رد شده توسط سیستم', class: 'bg-label-danger' },
        11: { text: 'رد شده توسط خیریه', class: 'bg-label-warning' }
    };

    // توابع کمکی برای فرمت‌دهی
    function formatDate(dateValue) {
        if (!dateValue) return '-';
        const date = new Date(dateValue);
        if (isNaN(date.getTime())) return '-';
        return date.toLocaleDateString('fa-IR');
    }

    function formatAmount(value) {
        if (value === null || value === undefined) return '-';
        return Number(value).toLocaleString('fa-IR') + ' ریال';
    }

    if (campaignId) {
        loadCampaignDetails();
    }

    function loadCampaignDetails() {
        $.ajax({
            url: apiUrl,
            type: 'GET',
            headers: {
                'Authorization': 'Bearer ' + token
            },
            success: function (response) {
                if (response && response.value) {
                    const campaign = response.value;

                    // پر کردن فیلدهای متنی
                    $('#campaign-title').text(campaign.title || 'بدون عنوان');
                    $('#campaign-description').text(campaign.description || 'توضیحاتی ثبت نشده است.');
                    $('#campaign-charity').text(campaign.charityName || 'نامشخص');

                    if (campaign.categories && campaign.categories.length > 0) {
                        const categoryNames = campaign.categories.map(c => c.name).join('، ');
                        $('#campaign-categories').text(categoryNames);
                    } else {
                        $('#campaign-categories').text('ثبت نشده');
                    }

                    // پر کردن فیلدهای شهر و مبالغ و تاریخ
                    $('#campaign-city').text(campaign.cityName || campaign.city?.name || '-');
                    $('#campaign-target-amount').text(formatAmount(campaign.totalAmount));
                    $('#campaign-start-date').text(formatDate(campaign.startDate));
                    $('#campaign-end-date').text(formatDate(campaign.endDate));

                    // مدیریت بنر کمپین
                    const bannerPath = campaign.bannerUrl || campaign.imagePath || campaign.bannerPath;
                    if (bannerPath) {
                        const fullImageUrl = bannerPath.startsWith('http') ? bannerPath : `${apiBaseUrl}${bannerPath}`;
                        $('#campaign-banner').attr('src', fullImageUrl).show();
                        $('#campaign-banner-placeholder').hide();
                    } else {
                        $('#campaign-banner').hide();
                        $('#campaign-banner-placeholder').show();
                    }

                    // نمایش badge وضعیت کمپین

                    const campaignStatus = campaign.campaignStatus ?? campaign.CampaignStatus;

                    const statusObj = statusMap[campaignStatus];
                    if (statusObj) {
                        $('#campaign-status').html(`<span class="badge ${statusObj.class}">${statusObj.text}</span>`);
                    } else {
                        $('#campaign-status').html(`<span class="badge bg-label-secondary">نامشخص (${campaignStatus})</span>`);
                    }

                    // فراخوانی رندر دکمه‌های عملیاتی ادمین/خیریه
                    renderAdminActions(campaignStatus);
                }
            },
            error: function () {
                Swal.fire('خطا', 'امکان دریافت اطلاعات کمپین وجود ندارد.', 'error');
            }
        });
    }






    function renderAdminActions(currentStatus) {
        const container = $('#status-buttons-container');
        const actionsCard = $('#admin-actions-card');

        container.empty();

        const status = Number(currentStatus);
        const parsedRole = Number(userRole);

        const isSystemAdmin = parsedRole === 1;
        const isCharityUser = parsedRole === 2 || parsedRole === 3;

        let hasContent = false;

        function addStatusButton(statusId, text, buttonClass) {
            container.append(`
            <button
                type="button"
                class="btn ${buttonClass} change-status-btn"
                data-status="${statusId}">
                ${text}
            </button>
        `);

            hasContent = true;
        }

        function addRestrictionMessage(message, alertClass) {
            container.html(`
            <div class="alert ${alertClass} mb-0" role="alert">
                ${message}
            </div>
        `);

            hasContent = true;
        }

        if (isSystemAdmin) {
            if (status === 1) {
                addStatusButton(2, 'تایید کمپین', 'btn-success');
                addStatusButton(3, 'رد کمپین', 'btn-danger');
            } else if (status === 2 || status === 5) {
                addStatusButton(8, 'تعلیق توسط سیستم', 'btn-warning');
            } else if (status === 8) {
                addStatusButton(5, 'فعال‌سازی مجدد کمپین', 'btn-info');
            } else if (status === 10) {
                addStatusButton(2, 'تایید مجدد کمپین', 'btn-success');
            } else if (status === 9) {
                addRestrictionMessage(
                    'این کمپین توسط خیریه تعلیق شده است و فقط کاربران خیریه می‌توانند آن را فعال کنند.',
                    'alert-warning'
                );
            } else if (status === 11) {
                addRestrictionMessage(
                    'این کمپین توسط خیریه رد شده است و فقط کاربران خیریه می‌توانند آن را تایید کنند.',
                    'alert-danger'
                );
            } else if (status === 3) {
                addStatusButton(2, 'تایید مجدد کمپین', 'btn-success');
            } else if (status === 4) {
                addStatusButton(8, 'تعلیق توسط سیستم', 'btn-warning');
                addStatusButton(5, 'فعال‌سازی مجدد کمپین', 'btn-info');
            }
        } else if (isCharityUser) {
            if (status === 1) {
                addStatusButton(2, 'تایید کمپین', 'btn-success');
                addStatusButton(3, 'رد کمپین', 'btn-danger');
            } else if (status === 2 || status === 5) {
                addStatusButton(9, 'تعلیق کمپین', 'btn-warning');
            } else if (status === 9) {
                addStatusButton(5, 'فعال‌سازی مجدد کمپین', 'btn-info');
            } else if (status === 8) {
                addRestrictionMessage(
                    'این کمپین توسط ادمین سیستم تعلیق شده است و فقط ادمین سیستم می‌تواند آن را فعال کند.',
                    'alert-danger'
                );
            } else if (status === 10) {
                addRestrictionMessage(
                    'این کمپین توسط ادمین سیستم رد شده است و فقط ادمین سیستم می‌تواند آن را تایید کند.',
                    'alert-danger'
                );
            } else if (status === 11) {
                addStatusButton(2, 'تایید مجدد کمپین', 'btn-success');
            } else if (status === 3) {
                addStatusButton(2, 'تایید مجدد کمپین', 'btn-success');
            } else if (status === 4) {
                addStatusButton(9, 'تعلیق توسط خیریه', 'btn-warning');
                addStatusButton(5, 'فعال‌سازی مجدد کمپین', 'btn-info');
            }
        }

        if (hasContent) {
            actionsCard.removeClass('d-none');
        } else {
            actionsCard.addClass('d-none');
        }

        $('.change-status-btn')
            .off('click')
            .on('click', function () {
                const newStatus = Number($(this).data('status'));

                Swal.fire({
                    title: 'تغییر وضعیت کمپین',
                    text: 'آیا مطمئن هستید که می‌خواهید وضعیت این کمپین را تغییر دهید؟',
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'بله، تغییر بده',
                    cancelButtonText: 'انصراف',
                    customClass: {
                        confirmButton: 'btn btn-primary me-3',
                        cancelButton: 'btn btn-label-secondary'
                    },
                    buttonsStyling: false
                }).then((result) => {
                    if (result.isConfirmed) {
                        executeStatusChange(newStatus);
                    }
                });
            });
    }








    function executeStatusChange(newStatus) {
        console.log("Sending status:", newStatus);

        $.ajax({
            url: `${apiBaseUrl}/api/campaign/${campaignId}/change-status`,
            type: 'PATCH',
            contentType: 'application/json',
            headers: {
                'Authorization': 'Bearer ' + token
            },
            data: JSON.stringify({ status: newStatus }),
            success: function () {
                console.log("Success!");
                Swal.fire({
                    title: 'موفقیت‌آمیز',
                    text: 'وضعیت کمپین با موفقیت تغییر یافت.',
                    icon: 'success',
                    confirmButtonText: 'باشه',
                    customClass: {
                        confirmButton: 'btn btn-primary'
                    },
                    buttonsStyling: false
                }).then(() => {
                    location.reload();
                });
            },
            error: function (xhr) {
                console.log("Error details:", xhr);
                const errorMsg = xhr.responseJSON?.errorMessage || 'خطایی در تغییر وضعیت رخ داد.';
                Swal.fire({
                    title: 'خطا',
                    text: errorMsg,
                    icon: 'error',
                    confirmButtonText: 'باشه',
                    customClass: {
                        confirmButton: 'btn btn-primary'
                    },
                    buttonsStyling: false
                });
            }
        });
    }
});
