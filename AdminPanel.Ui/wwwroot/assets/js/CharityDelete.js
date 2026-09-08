/**
 * Charity Management - Delete Logic
 */

$(document).ready(function () {
    // مقداردهی اولیه مودال بوت‌استرپ
    var deleteModalElement = document.getElementById('deleteCharityModal');
    var deleteModal = null;

    if (deleteModalElement) {
        deleteModal = new bootstrap.Modal(deleteModalElement);
    }

    // ۱. هندل کردن کلیک روی دکمه حذف در جدول (باز شدن مودال)
    $(document).on('click', '.btn-delete-charity', function (e) {
        e.preventDefault();
        e.stopPropagation();

        // خواندن آی‌دی از دیتای دکمه
        var charityId = $(this).data('id');

        if (!charityId) {
            console.error('شناسه خیریه در دکمه یافت نشد.');
            return;
        }

        // قرار دادن آی‌دی در فیلد مخفی داخل مودال
        $('#deleteCharityId').val(charityId);

        // نمایش مودال تایید
        if (deleteModal) {
            deleteModal.show();
        }
    });

    // ۲. هندل کردن دکمه تایید نهایی حذف (ارسال درخواست به سرور)
    $('#btnConfirmDeleteCharity').on('click', function () {
        var charityId = $('#deleteCharityId').val();
        var token = $('input[name="__RequestVerificationToken"]').val();

        if (!charityId) {
            console.error('شناسه خیریه نامعتبر است.');
            return;
        }

        // تغییر وضعیت دکمه به حالت در حال لودینگ
        $('#btnConfirmDeleteCharity')
            .prop('disabled', true)
            .text('در حال حذف...');

        console.log("ارسال درخواست حذف برای آی‌دی:", charityId);

        $.ajax({
            url: '/Management/Charities?handler=DeleteCharity',
            type: 'POST',
            data: {
                id: charityId
            },
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                console.log("پاسخ سرور:", response);

                if (response && response.success) {
                    // بستن مودال در صورت موفقیت
                    if (deleteModal) {
                        deleteModal.hide();
                    }
                    // ریلود کردن جدول بدون رفرش صفحه
                    reloadCharitiesTable();
                } else {
                    // نمایش خطا در کنسول (می‌توانید اینجا Toast یا Alert بگذارید)
                    console.error("خطا از سمت سرور:", response?.message || "خطای نامشخص");
                }
            },
            error: function (xhr) {
                console.error("خطای ارتباطی (AJAX Error):", xhr.status, xhr.statusText);
            },
            complete: function () {
                // فعال کردن مجدد دکمه در هر حالت (موفقیت یا شکست)
                $('#btnConfirmDeleteCharity')
                    .prop('disabled', false)
                    .text('حذف');
            }
        });
    });

    // تابع کمکی برای ریلود کردن DataTable
    function reloadCharitiesTable() {
        var table = $('.dt-advanced-search').DataTable();
        if (table) {
            table.ajax.reload(null, false);
        }
    }
});
