'use strict';

$(function () {
    const $form = $('#editCampaignForm');
    const token = $('meta[name="access-token"]').attr('content');
    const userType = parseInt($('meta[name="user-type"]').attr('content') || "0");
    const userCharityId = $('meta[name="user-charity-id"]').attr('content');
    const campaignId = $('#campaign-id').val();

    const $bannerPreview = $('#campaign-banner-preview');
    const $bannerPlaceholder = $('#campaign-banner-placeholder');
    const $removeBannerBtn = $('#remove-campaign-banner-btn');
    const $removeBannerHidden = $('#remove-campaign-banner');
    const $bannerIdHidden = $('#edit-campaign-banner-id');
    const $bannerFileInput = $('#campaign-banner-input');

    const campaignStatusMap = {
        1: { name: 'ایجاد شده', badge: 'bg-info' },
        2: { name: 'تایید شده', badge: 'bg-success' },
        3: { name: 'رد شده', badge: 'bg-danger' },
        4: { name: 'تعلیق شده', badge: 'bg-warning text-dark' },
        5: { name: 'فعال‌سازی مجدد', badge: 'bg-primary' },
        6: { name: 'تأمین مالی موفق', badge: 'bg-success' },
        7: { name: 'تأمین مالی ناموفق', badge: 'bg-secondary' },
        8: { name: 'تعلیق توسط سیستم', badge: 'bg-danger' },
        9: { name: 'تعلیق توسط خیریه', badge: 'bg-warning text-dark' },
        10: { name: 'رد شده توسط سیستم', badge: 'bg-danger' },
        11: { name: 'رد شده توسط خیریه', badge: 'bg-warning text-dark' }
    };

    const charityTransitions = {
        // Created -> Approved / Rejected
        1: [2, 3],

        // Approved -> SuspendedByCharity
        2: [9],

        // Reactivated -> SuspendedByCharity
        5: [9],

        // SuspendedByCharity -> Reactivated
        9: [5],

        // Legacy Suspended
        4: [9, 5],

        // RejectedByCharity -> Approved
        11: [2],

        // RejectedBySystem: charity cannot approve it
        10: []
    };


    const systemTransitions = {
        // Created -> Approved / Rejected
        1: [2, 3],

        // Legacy Rejected
        3: [2],

        // Approved -> SuspendedBySystem
        2: [8],

        // Reactivated -> SuspendedBySystem
        5: [8],

        // SuspendedBySystem -> Reactivated
        8: [5],

        // Legacy Suspended
        4: [8, 5],

        // RejectedBySystem -> Approved
        10: [2],

        // RejectedByCharity: system cannot approve it
        11: []
    };


    const activeStatuses = [2, 5];

    let initialCityId = null;
    let initialCategoryIds = [];
    let initialCharityId = null;

    function setBannerState_HasBanner(fullUrl) {
        $bannerPreview.attr('src', fullUrl).show();
        $bannerPlaceholder.hide();
        $removeBannerBtn.show();
        if ($removeBannerHidden.length) $removeBannerHidden.val('false');
    }

    function setBannerState_NoBanner() {
        $bannerPreview.attr('src', '').hide();
        $bannerPlaceholder.show();
        $removeBannerBtn.hide();
    }

    function markBannerForRemoval() {
        setBannerState_NoBanner();
        if ($removeBannerHidden.length) $removeBannerHidden.val('true');
        $bannerIdHidden.val('');
        if ($bannerFileInput.length) $bannerFileInput.val('');
    }

    function checkAndToggleStatusAlert(statusId) {
        const currentStatus = parseInt(statusId || "0");
        const isRestrictedUser = userType !== 1;

        if (isRestrictedUser && activeStatuses.includes(currentStatus)) {
            const statusName = campaignStatusMap[currentStatus]?.name || 'فعال';
            $('#alert-status-name').text(statusName);
            $('#campaign-status-alert').css('display', 'flex');
            return;
        }

        $('#campaign-status-alert').hide();
    }

    $bannerFileInput.on('change', function () {
        const hasFile = this.files && this.files.length > 0;
        if (hasFile) {
            if ($removeBannerHidden.length) $removeBannerHidden.val('false');
            const file = this.files[0];
            const localUrl = URL.createObjectURL(file);
            setBannerState_HasBanner(localUrl);
        }
    });

    $removeBannerBtn.on('click', function () {
        Swal.fire({
            title: 'حذف بنر؟',
            text: 'بنر فعلی کمپین حذف می‌شود. می‌توانید بعداً بنر جدید انتخاب کنید.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'بله، حذف شود',
            cancelButtonText: 'انصراف'
        }).then((res) => {
            if (res.isConfirmed) {
                markBannerForRemoval();
            }
        });
    });

    const startDatePicker = $('#campaign-start-date-display').persianDatepicker({
        format: 'YYYY/MM/DD',
        altField: '#campaign-start-date',
        altFormat: 'X',
        autoClose: true,
        initialValue: false,
        onSelect: function (unix) {
            const miladiDate = new persianDate(unix).toDate();
            $('#campaign-start-date').val(miladiDate.toISOString());
        }
    });

    const endDatePicker = $('#campaign-end-date-display').persianDatepicker({
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

    $('#campaign-city').select2({
        placeholder: 'انتخاب شهر',
        language: "fa",
        dir: "rtl"
    });

    $('#campaign-category').select2({
        placeholder: 'دسته‌بندی‌های کمپین را انتخاب کنید',
        language: "fa",
        dir: "rtl"
    });

    function loadCities(callback) {
        $.ajax({
            url: `${apiBaseUrl}/api/city`,
            type: 'GET',
            headers: { Authorization: `Bearer ${token}` },
            success: function (response) {
                const cities = (response && response.value) ? response.value : [];
                let optionsHtml = '<option value="">انتخاب شهر...</option>';
                cities.forEach(city => {
                    optionsHtml += `<option value="${city.id}">${city.name}</option>`;
                });
                $('#campaign-city').html(optionsHtml).trigger('change');
                if (callback) callback();
            },
            error: function () {
                console.error("خطا در دریافت لیست شهرها");
            }
        });
    }


    function loadCategoriesForCharity(charityId, callback) {
        if (!charityId) {
            $('#campaign-category').empty().trigger('change');
            if (callback) callback();
            return;
        }

        $.ajax({
            url: `${apiBaseUrl}/api/category/ByCharity/${charityId}`,
            type: 'GET',
            headers: { Authorization: `Bearer ${token}` },
            success: function (response) {
                const categories = (response && response.value) ? response.value : [];
                let optionsHtml = '';

                categories.forEach(category => {
                    const categoryId = category.id ?? category.Id;
                    const categoryName = category.name ?? category.Name;

                    optionsHtml += `<option value="${categoryId}">${categoryName}</option>`;
                });

                $('#campaign-category').html(optionsHtml).trigger('change');

                if (callback) callback();
            },
            error: function () {
                console.error("خطا در دریافت لیست دسته‌بندی‌های مجاز خیریه");
            }
        });
    }


    function normalizeStatusId(statusId) {
        const parsedStatusId = parseInt(statusId || "0", 10);

        if (parsedStatusId === 4) {
            return userType === 1 ? 8 : 9;
        }

        return parsedStatusId;
    }




    function renderStatusManagement(statusId) {
        const rawStatusId = parseInt(statusId || "0", 10);
        const normalizedStatusId = normalizeStatusId(rawStatusId);
        const isSystemAdmin = userType === 1;
        const isCharityUser = userType === 2 || userType === 3;

        const transitions = isSystemAdmin
            ? systemTransitions
            : isCharityUser
                ? charityTransitions
                : {};


        const statusInfo = campaignStatusMap[normalizedStatusId] || {
            name: 'نامشخص',
            badge: 'bg-secondary'
        };

        $('#current-status-badge')
            .text(statusInfo.name)
            .removeClass()
            .addClass(`badge ${statusInfo.badge} fs-7`);

        checkAndToggleStatusAlert(normalizedStatusId);

        const $container = $('#status-transitions-container');
        $container.empty();

        const targets = transitions[rawStatusId] || [];

        if (targets.length === 0) {
            let message = 'انتقال مجاز دیگری در این وضعیت وجود ندارد.';

            if (!isSystemAdmin && rawStatusId === 10) {
                message = 'این کمپین توسط سیستم رد شده و فقط ادمین سیستم می‌تواند آن را تایید مجدد کند.';
            }

            if (isSystemAdmin && rawStatusId === 11) {
                message = 'این کمپین توسط خیریه رد شده و فقط کاربران خیریه می‌توانند آن را تایید مجدد کنند.';
            }

            if (!isSystemAdmin && rawStatusId === 8) {
                message = 'این کمپین توسط ادمین سیستم تعلیق شده و فقط ادمین سیستم می‌تواند آن را فعال کند.';
            }

            if (isSystemAdmin && rawStatusId === 9) {
                message = 'این کمپین توسط خیریه تعلیق شده و فقط کاربران خیریه می‌توانند آن را فعال کنند.';
            }

            $container.html(
                `<span class="text-muted small">${message}</span>`
            );
        } else {
            targets.forEach(targetId => {
                const targetInfo = campaignStatusMap[targetId];

                if (!targetInfo) {
                    return;
                }

                let btnClass = 'btn-outline-primary';

                if (targetId === 2) {
                    btnClass = 'btn-outline-success';
                } else if (targetId === 3 || targetId === 10 || targetId === 11) {
                    btnClass = 'btn-outline-danger';
                } else if (targetId === 8 || targetId === 9) {
                    btnClass = 'btn-outline-warning text-dark';
                } else if (targetId === 5) {
                    btnClass = 'btn-outline-primary';
                }

                const $btn = $('<button>', {
                    type: 'button',
                    class: `btn ${btnClass} btn-sm fw-bold`,
                    text: `تغییر به ${targetInfo.name}`
                });

                $btn.on('click', function () {
                    changeCampaignStatusDirectly(targetId, targetInfo.name);
                });

                $container.append($btn);
            });
        }

        $('#charity-status-management-card').stop(true, true).slideDown();
    }



    function changeCampaignStatusDirectly(newStatusId, newStatusName) {
        Swal.fire({
            title: 'تغییر وضعیت کمپین؟',
            text: `آیا از تغییر وضعیت کمپین به "${newStatusName}" اطمینان دارید؟`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'بله، تغییر بده',
            cancelButtonText: 'انصراف'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `${apiBaseUrl}/api/campaign/${campaignId}/change-status`,
                    type: 'PATCH',
                    contentType: 'application/json',
                    headers: { Authorization: `Bearer ${token}` },
                    data: JSON.stringify({ status: newStatusId }),
                    success: function (response) {
                        if (response.hasError) {
                            Swal.fire('خطا!', response.errorMessage || 'خطا در تغییر وضعیت کمپین', 'error');
                            return;
                        }

                        const actualStatusId = normalizeStatusId(newStatusId);

                        Swal.fire({
                            title: 'موفقیت‌آمیز',
                            text: `وضعیت کمپین با موفقیت به "${newStatusName}" تغییر یافت.`,
                            icon: 'success',
                            customClass: { confirmButton: 'btn btn-success' }
                        }).then(() => {
                            $('#edit-campaign-status').val(actualStatusId);

                            renderStatusManagement(actualStatusId);
                            checkAndToggleStatusAlert(actualStatusId);
                        });

                    },
                    error: function (xhr) {
                        let msg = xhr.responseJSON?.errorMessage || 'امکان تغییر وضعیت در این مرحله وجود ندارد.';
                        Swal.fire('خطا!', msg, 'error');
                    }
                });
            }
        });
    }

    function loadCampaignDetails() {
        console.log("Loading campaign with ID: " + campaignId);

        $.ajax({
            url: `${apiBaseUrl}/api/campaign/${campaignId}`,
            type: 'GET',
            headers: { Authorization: `Bearer ${token}` },
            success: function (response) {
                if (response && response.value) {
                    const campaign = response.value;
                    const campaignStatus = campaign.campaignStatus ?? campaign.CampaignStatus;

                    $('#campaign-title').val(campaign.title ?? campaign.Title ?? '');
                    $('#campaign-total-amount').val(campaign.totalAmount ?? campaign.TotalAmount ?? '');
                    $('#campaign-description').val(campaign.description ?? campaign.Description ?? '');
                    $('#edit-campaign-status').val(normalizeStatusId(campaignStatus));

                    renderStatusManagement(campaignStatus);
                    checkAndToggleStatusAlert(campaignStatus);

                    $bannerIdHidden.val(campaign.bannerId ?? '');
                    if ($removeBannerHidden.length) $removeBannerHidden.val('false');

                    initialCityId = campaign.cityId ?? campaign.CityId;
                    initialCategoryIds = (campaign.categories ?? campaign.Categories ?? []).map(c => c.id ?? c.Id);
                    initialCharityId = campaign.charityId ?? campaign.CharityId;

                    if (initialCityId) {
                        $('#campaign-city').val(initialCityId).trigger('change');
                    }

                    const bannerUrl = campaign.bannerUrl ?? campaign.BannerUrl;
                    if (bannerUrl) {
                        const fullImageUrl = bannerUrl.startsWith('http')
                            ? bannerUrl
                            : `${apiBaseUrl}${bannerUrl}`;

                        setBannerState_HasBanner(fullImageUrl);
                    } else {
                        setBannerState_NoBanner();
                    }

                    if (campaign.startDate) {
                        const startUnix = new Date(campaign.startDate).getTime();
                        startDatePicker.setDate(startUnix);
                        $('#campaign-start-date').val(campaign.startDate);
                    }

                    if (campaign.endDate) {
                        const endUnix = new Date(campaign.endDate).getTime();
                        endDatePicker.setDate(endUnix);
                        $('#campaign-end-date').val(campaign.endDate);
                    }

                    if (userType === 1) {
                        const option = new Option(campaign.charityName, campaign.charityId, true, true);
                        $('#campaign-charity').append(option).trigger('change');

                        loadCategoriesForCharity(campaign.charityId, function () {
                            $('#campaign-category').val(initialCategoryIds).trigger('change');
                        });
                    } else {
                        loadCategoriesForCharity(userCharityId, function () {
                            $('#campaign-category').val(initialCategoryIds).trigger('change');
                        });
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error, xhr?.responseText);
                Swal.fire('خطا', 'امکان دریافت اطلاعات کمپین برای ویرایش وجود ندارد.', 'error');
            }
        });
    }

    loadCities(function () {
        if (userType === 1) {
            $('#campaign-charity').select2({
                placeholder: 'انتخاب خیریه هدف',
                language: "fa",
                dir: "rtl",
                ajax: {
                    url: `${apiBaseUrl}/api/charity`,
                    dataType: 'json',
                    headers: { Authorization: `Bearer ${token}` },
                    processResults: function (data) {
                        const items = data.value.items || data.value;
                        return {
                            results: items.map(c => ({ id: c.id, text: c.name }))
                        };
                    }
                }
            });

            $('#campaign-charity').on('change', function () {
                const selectedCharityId = $(this).val();
                loadCategoriesForCharity(selectedCharityId);
            });
        }

        loadCampaignDetails();
    });

    $form.on('submit', function (e) {
        e.preventDefault();

        const currentStatusVal = parseInt($('#edit-campaign-status').val() || "0");
        if (userType !== 1 && activeStatuses.includes(currentStatusVal)) {
            Swal.fire({
                title: 'خطای ذخیره‌سازی!',
                text: 'این کمپین در وضعیت فعال قرار دارد و امکان ثبت تغییرات برای آن وجود ندارد. لطفاً ابتدا وضعیت کمپین را تغییر دهید.',
                icon: 'error',
                confirmButtonText: 'متوجه شدم'
            });
            return;
        }

        const $btn = $('#saveCampaignBtn');
        $btn.prop('disabled', true).text('در حال بروزرسانی کمپین...');

        const startDateVal = $('#campaign-start-date').val();
        const endDateVal = $('#campaign-end-date').val();

        if (!startDateVal || !endDateVal) {
            Swal.fire('هشدار!', 'لطفاً تاریخ شروع و تاریخ پایان را وارد نمایید.', 'warning');
            $btn.prop('disabled', false).text('ذخیره تغییرات');
            return;
        }

        const start = new Date(startDateVal);
        const end = new Date(endDateVal);
        if (start >= end) {
            Swal.fire('خطا در تاریخ!', 'تاریخ پایان کمپین باید بعد از تاریخ شروع آن باشد.', 'error');
            $btn.prop('disabled', false).text('ذخیره تغییرات');
            return;
        }

        const formData = new FormData(this);

        formData.delete('CategoryIds');
        const selectedCategories = $('#campaign-category').val();
        if (selectedCategories && selectedCategories.length > 0) {
            selectedCategories.forEach((id, index) => {
                formData.append(`CategoryIds[${index}]`, id);
            });
        }

        if (userType !== 1 && userCharityId) {
            formData.set('CharityId', userCharityId);
        }

        if ($removeBannerHidden.length) {
            formData.set('RemoveBanner', $removeBannerHidden.val() === 'true' ? 'true' : 'false');
        }

        $.ajax({
            url: `${apiBaseUrl}/api/campaign`,
            type: 'PUT',
            data: formData,
            processData: false,
            contentType: false,
            headers: {
                Authorization: `Bearer ${token}`
            },
            success: function (response) {
                if (response.hasError) {
                    Swal.fire('خطا!', response.errorMessage || 'خطا در ویرایش کمپین', 'error');
                    return;
                }

                Swal.fire({
                    title: 'موفقیت!',
                    text: 'تغییرات کمپین با موفقیت ذخیره شد.',
                    icon: 'success',
                    customClass: { confirmButton: 'btn btn-success' }
                }).then(() => {
                    window.location.href = campaignListUrl;
                });
            },
            error: function (xhr) {
                let msg = xhr.responseJSON?.errorMessage || 'خطا در ارتباط با سرور و ویرایش اطلاعات';
                Swal.fire('خطا!', msg, 'error');
            },
            complete: function () {
                $btn.prop('disabled', false).text('ذخیره تغییرات');
            }
        });
    });
});
