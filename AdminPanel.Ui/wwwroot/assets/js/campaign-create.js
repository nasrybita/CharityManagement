'use strict';

$(function () {
    const apiBaseUrl = 'https://localhost:7209'; // API Address
    const $form = $('#createCampaignForm');
    const token = $('meta[name="access-token"]').attr('content');
    const userType = parseInt($('meta[name="user-type"]').attr('content') || "0");
    const userCharityId = $('meta[name="user-charity-id"]').attr('content');

    // فعال‌سازی تقویم شمسی برای تاریخ شروع
    $('#campaign-start-date-display').persianDatepicker({
        format: 'YYYY/MM/DD',
        altField: '#campaign-start-date', // مقدار خروجی در فیلد مخفی ذخیره شود
        altFormat: 'X', // مقدار به صورت Timestamp ذخیره شود تا راحت تبدیل به میلادی شود
        autoClose: true,
        initialValue: false,
        onSelect: function (unix) {
            // تبدیل تاریخ شمسی انتخاب شده به ISO String میلادی جهت ارسال به API
            const miladiDate = new persianDate(unix).toDate();
            $('#campaign-start-date').val(miladiDate.toISOString());
        }
    });

    // فعال‌سازی تقویم شمسی برای تاریخ پایان
    $('#campaign-end-date-display').persianDatepicker({
        format: 'YYYY/MM/DD',
        altField: '#campaign-end-date',
        altFormat: 'X',
        autoClose: true,
        initialValue: false,
        onSelect: function (unix) {
            const miladiDate = new persianDate(unix).toDate();
            $('#campaign-end-date').val(miladiDate.toISOString());
        }
    });

    // Enable Select2 for the city
    $('#campaign-city').select2({
        placeholder: 'انتخاب شهر',
        language: "fa",
        dir: "rtl"
    });

    // Load cities from API
    function loadCities() {
        $.ajax({
            url: `${apiBaseUrl}/api/city`,
            type: 'GET',
            headers: {
                Authorization: `Bearer ${token}`
            },
            success: function (response) {
                const cities = (response && response.value) ? response.value : [];

                let optionsHtml = '<option value="">انتخاب شهر...</option>';
                cities.forEach(city => {
                    optionsHtml += `<option value="${city.id}">${city.name}</option>`;
                });

                $('#campaign-city').html(optionsHtml).trigger('change');
            },
            error: function () {
                console.error("خطا در دریافت لیست شهرها");
                Swal.fire({
                    title: 'خطا!',
                    text: 'دریافت لیست شهرها با مشکل مواجه شد.',
                    icon: 'error',
                    customClass: { confirmButton: 'btn btn-primary' }
                });
            }
        });
    }

    // Load categories based on the charity ID
    function loadCategoriesForCharity(charityId) {
        if (!charityId) {
            $('#campaign-category').empty().trigger('change');
            return;
        }

        // Retrieve the category list from the API
        $.ajax({
            url: `${apiBaseUrl}/api/category/ByCharity/${charityId}`,
            type: 'GET',
            headers: {
                Authorization: `Bearer ${token}`
            },
            success: function (response) {
                const categories = (response && response.value) ? response.value : [];

                let optionsHtml = '';
                categories.forEach(category => {
                    optionsHtml += `<option value="${category.id}">${category.name}</option>`;
                });

                $('#campaign-category').html(optionsHtml).trigger('change');
            },
            error: function () {
                console.error("خطا در دریافت لیست دسته‌بندی‌های مجاز خیریه");
            }
        });
    }

    // Initialize Select2 for categories
    $('#campaign-category').select2({
        placeholder: 'دسته‌بندی‌های کمپین را انتخاب کنید',
        language: "fa",
        dir: "rtl"
    });

    // Load cities on page load
    loadCities();

    // Check the user type to load charity information
    if (userType === 1) { // AdminSystem
        // Enable Select2 for charities with server-side search
        $('#campaign-charity').select2({
            placeholder: 'انتخاب خیریه هدف',
            language: "fa",
            dir: "rtl",
            ajax: {
                url: `${apiBaseUrl}/api/charity`,
                dataType: 'json',
                headers: {
                    Authorization: `Bearer ${token}`
                },
                processResults: function (data) {
                    const items = data.value.items || data.value;
                    return {
                        results: items.map(c => ({ id: c.id, text: c.name }))
                    };
                }
            }
        });

        // Reload the category list when the system admin changes the selected charity
        $('#campaign-charity').on('change', function () {
            const selectedCharityId = $(this).val();
            loadCategoriesForCharity(selectedCharityId);
        });

    } else if ((userType === 2 || userType === 3) && userCharityId) {
        // CharityAdmin and CharityUser can only see the categories associated with their own charity 
        loadCategoriesForCharity(userCharityId);
    }

    // Handle form submission
    $form.on('submit', function (e) {
        e.preventDefault();

        const $btn = $('#saveCampaignBtn');
        $btn.prop('disabled', true).text('در حال ارسال کمپین...');

        // خواندن مقادیر میلادی تاریخ شروع و پایان برای اعتبارسنجی
        const startDateVal = $('#campaign-start-date').val();
        const endDateVal = $('#campaign-end-date').val();

        // بررسی پر بودن فیلدهای تاریخ
        if (!startDateVal || !endDateVal) {
            Swal.fire({
                title: 'هشدار!',
                text: 'لطفاً تاریخ شروع و تاریخ پایان کمپین را وارد نمایید.',
                icon: 'warning',
                customClass: { confirmButton: 'btn btn-primary' }
            });
            $btn.prop('disabled', false).text('ثبت کمپین');
            return;
        }

        // بررسی بزرگتر بودن تاریخ پایان از تاریخ شروع
        const start = new Date(startDateVal);
        const end = new Date(endDateVal);
        if (start >= end) {
            Swal.fire({
                title: 'خطا در تاریخ!',
                text: 'تاریخ پایان کمپین باید بعد از تاریخ شروع آن باشد.',
                icon: 'error',
                customClass: { confirmButton: 'btn btn-primary' }
            });
            $btn.prop('disabled', false).text('ثبت کمپین');
            return;
        }

        const formData = new FormData(this);

        // Prepare the category array to correctly match the List<int> structure
        formData.delete('CategoryIds');
        const selectedCategories = $('#campaign-category').val();
        if (selectedCategories && selectedCategories.length > 0) {
            selectedCategories.forEach((id, index) => {
                formData.append(`CategoryIds[${index}]`, id);
            });
        }

        // If the user is a charity admin, ensure that CharityId is submitted with their own charity ID
        if (userType !== 1 && userCharityId) {
            formData.set('CharityId', userCharityId);
        }

        $.ajax({
            url: `${apiBaseUrl}/api/campaign`,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            headers: {
                Authorization: `Bearer ${token}`
            },
            success: function (response) {
                if (response.hasError) {
                    Swal.fire({
                        title: 'خطا!',
                        text: response.errorMessage || 'خطا در ثبت کمپین جدید',
                        icon: 'error',
                        customClass: { confirmButton: 'btn btn-primary' }
                    });
                    return;
                }

                Swal.fire({
                    title: 'موفقیت!',
                    text: 'کمپین جدید با موفقیت ثبت و تعریف شد.',
                    icon: 'success',
                    customClass: { confirmButton: 'btn btn-success' }
                }).then(() => {
                    window.location.href = campaignListUrl;
                });
            },
            error: function (xhr) {
                let msg = xhr.responseJSON?.errorMessage || 'خطا در برقراری ارتباط با سرور و ایجاد کمپین';
                Swal.fire({
                    title: 'خطا!',
                    text: msg,
                    icon: 'error',
                    customClass: { confirmButton: 'btn btn-primary' }
                });
            },
            complete: function () {
                $btn.prop('disabled', false).text('ثبت کمپین');
            }
        });
    });
});
