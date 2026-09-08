'use strict';

$(function () {
    const baseUrl =
        window.apiBaseUrl ||
        (typeof apiBaseUrl !== 'undefined' ? apiBaseUrl : 'https://localhost:7209');

    const accessToken = $('meta[name="access-token"]').attr('content');
    //const currentUserIsRoot =
    //    typeof isCurrentUserRoot !== 'undefined' && isCurrentUserRoot === true;

    const currentUserIsRoot = window.isCurrentUserRoot === true;



    const currentUserType = parseInt(
        $('meta[name="user-type"]').attr('content')
    );

    const currentUserId = parseInt(
        $('meta[name="current-user-id"]').attr('content')
    );

    const USER_TYPES = {
        ADMIN_SYSTEM: 1,
        CHARITY_ADMIN: 2,
        CHARITY_USER: 3
    };



    function canResetPassword(row) {
        const rowId = Number(row.id);

        if (rowId === Number(currentUserId)) {
            return true;
        }

        // SystemAdmin می‌تواند رمز همه (از جمله سایر SystemAdmin ها) را تغییر دهد
        if (currentUserType === USER_TYPES.ADMIN_SYSTEM) {
            return true;
        }

        if (
            currentUserType === USER_TYPES.CHARITY_ADMIN &&
            row.userType === USER_TYPES.CHARITY_USER
        ) {
            return true;
        }

        return false;
    }






    const $tableEl = $('.datatables-users');
    const $editForm = $('#editUserForm');
    const $editModal = $('#editUserModal');
    const $editUserType = $('#editUserType');
    const $editUserCharity = $('#editUserCharity');
    const $editUserCharityWrapper = $('#editUserCharityWrapper');
    const $rootOnlyFields = $('.root-only-field');

    function getAuthHeaders() {
        return accessToken ? { Authorization: `Bearer ${accessToken}` } : {};
    }

    function roleText(value) {
        switch (value) {
            case 1:
                return 'مدیر سیستم';
            case 2:
                return 'مدیر خیریه';
            case 3:
                return 'کاربر';
            default:
                return 'نامشخص';
        }
    }

    function formatDate(value) {
        if (!value) return '';
        const date = new Date(value);
        if (isNaN(date.getTime())) return '';
        return date.toLocaleDateString('fa-IR');
    }

    function escapeAttr(value) {
        return String(value ?? '')
            .replaceAll('&', '&amp;')
            .replaceAll("'", '&apos;')
            .replaceAll('"', '&quot;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;');
    }

    function showError(message) {
        Swal.fire({
            title: 'خطا!',
            text: message,
            icon: 'error',
            customClass: { confirmButton: 'btn btn-primary' },
            buttonsStyling: false
        });
    }

    function toggleEditCharityByUserType(userType) {
        if (!currentUserIsRoot) {
            $editUserCharityWrapper.addClass('d-none');
            return;
        }

        if (parseInt(userType, 10) === USER_TYPES.ADMIN_SYSTEM) {
            $editUserCharityWrapper.addClass('d-none');
            $editUserCharity.val('');
            return;
        }

        $editUserCharityWrapper.removeClass('d-none');
    }




    function loadUserTypes(selectedValue) {
        return $.ajax({
            url: `${baseUrl}/api/User/user-types`,
            type: 'GET',
            headers: getAuthHeaders()
        }).done(function (response) {

            console.log('user types response:', response);

            const items =
                response?.value ||
                response?.Value ||
                response;

            $editUserType.empty();

            $editUserType.append(
                '<option value="">نوع دسترسی را انتخاب کنید</option>'
            );

            items.forEach(function (item) {

                const id = item.id ?? item.Id;
                const title = item.title ?? item.Title;

                if (id != null && title) {
                    $editUserType.append(
                        new Option(title, id, false, false)
                    );
                }
            });


            if (selectedValue != null) {
                $editUserType.val(String(selectedValue));
                toggleEditCharityByUserType(selectedValue);
            }

        });
    }




    function loadCharities(selectedValue) {
        return $.ajax({
            url: `${baseUrl}/api/Charity`,
            type: 'GET',
            data: { Page: 1, PageSize: 100 },
            headers: getAuthHeaders()
        }).done(function (response) {
            const items = response?.value?.items || response?.value?.Items || [];

            $editUserCharity.empty();
            $editUserCharity.append('<option value="">انتخاب خیریه</option>');

            items.forEach(function (item) {
                const id = item.id ?? item.Id;
                const name = item.name ?? item.Name;

                if (id != null && name) {
                    $editUserCharity.append(new Option(name, id, false, false));
                }
            });

            if (selectedValue != null) {
                $editUserCharity.val(String(selectedValue));
            }
        });
    }

    const dataTable = $tableEl.DataTable({
        processing: true,
        serverSide: false,
        ajax: {
            url: `${baseUrl}/api/User`,
            type: 'GET',
            headers: getAuthHeaders(),
            dataSrc: function (json) {
                if (!json || json.hasError || !json.value) return [];
                return json.value;
            }
        },
        columns: [
            { data: null },
            { data: 'name' },
            { data: 'userName' },
            { data: 'mobile' },
            { data: 'charityName' },
            { data: 'userType' },
            { data: 'createdAt' },
            { data: null }
        ],
        columnDefs: [
            {
                targets: 0,
                orderable: false,
                searchable: false,
                render: function (data, type, row) {
                    return '<div class="form-check d-flex justify-content-center">' +
                        `<input class="form-check-input dt-checkboxes" type="checkbox" value="${row.id}">` +
                        '</div>';
                }
            },
            {
                targets: 4,
                render: function (data) {
                    return data ? data : '---';
                }
            },
            {
                targets: 5,
                render: function (data) {
                    return roleText(data);
                }
            },
            {
                targets: 6,
                render: function (data) {
                    return formatDate(data);
                }
            },
            {
                targets: 7,
                orderable: false,
                searchable: false,
                render: function (data, type, row) {

                    let resetPasswordButton = '';

                    const userData = escapeAttr(JSON.stringify(row));


                    if (canResetPassword(row)) {

                        resetPasswordButton = `
            <a href="javascript:;"
               class="dropdown-item user-reset-password"
               data-id="${row.id}"
               data-name="${escapeAttr(row.name)}">

                <i class="ti ti-key me-2"></i>
                تغییر رمز عبور

            </a>
        `;
                    }


                    return `


        <div class="d-inline-block">

            <a href="javascript:;"
               class="btn btn-sm btn-icon dropdown-toggle hide-arrow"
               data-bs-toggle="dropdown">

                <i class="ti ti-dots-vertical"></i>

            </a>


            <div class="dropdown-menu dropdown-menu-end m-0">

                <a href="javascript:;"
                   class="dropdown-item user-edit"
                   data-user='${userData}'>

                    <i class="ti ti-pencil me-2"></i>
                    ویرایش

                </a>


                <a href="javascript:;"
                   class="dropdown-item user-details"
                   data-user='${userData}'>

                    <i class="ti ti-eye me-2"></i>
                    جزئیات

                </a>


                ${resetPasswordButton}


                <a href="javascript:;"
                   class="dropdown-item text-danger user-delete"
                   data-id="${row.id}"
                   data-name="${escapeAttr(row.name)}">

                    <i class="ti ti-trash me-2"></i>
                    حذف

                </a>


            </div>

        </div>
    `;
                }
            }
        ],
        order: [[6, 'desc']]
    });

    if (currentUserIsRoot) {
        $rootOnlyFields.removeClass('d-none');

        //loadUserTypes();
        //loadCharities();


        loadUserTypes()
            .then(function () {
                console.log('User types loaded');
            });

        loadCharities();

        $editUserType.on('change', function () {
            toggleEditCharityByUserType($(this).val());
        });
    }

    $tableEl.find('tbody').on('click', '.user-details', function () {
        const data = $(this).attr('data-user');
        if (!data) return;

        const user = JSON.parse(data);

        $('#det-name').val(user.name || '');
        $('#det-username').val(user.userName || '');
        $('#det-mobile').val(user.mobile || '');
        $('#det-usertype').val(roleText(user.userType));
        $('#det-charity').val(user.userType === USER_TYPES.ADMIN_SYSTEM ? '---' : (user.charityName || '---'));
        $('#det-created').val(formatDate(user.createdAt));

        $('#userDetailsModal').modal('show');
    });

    $tableEl.find('tbody').on('click', '.user-edit', function () {
        const data = $(this).attr('data-user');
        if (!data) return;

        const user = JSON.parse(data);

        //test
        console.log(user);
        //test

        $('#editUserId').val(user.id);
        $('#editUserName').val(user.name || '');
        $('#editUserUserName').val(user.userName || '');
        $('#editUserMobile').val(user.mobile || '');
        $('#editUserSex').val(String(user.sex));

        if (currentUserIsRoot) {

            loadUserTypes(user.userType);

            toggleEditCharityByUserType(user.userType);

            if (user.userType === USER_TYPES.ADMIN_SYSTEM) {
                $editUserCharity.val('');
            }
            else {
                $editUserCharity.val(
                    user.charityId != null
                        ? String(user.charityId)
                        : ''
                );
            }
        }

        $('#editUserModal').modal('show');
    });

    $editForm.on('submit', function (e) {
        e.preventDefault();

        const id = parseInt($('#editUserId').val(), 10);
        const $submitBtn = $('#updateUserBtn');

        if (!id || id <= 0) {
            showError('شناسه کاربر معتبر نیست.');
            return;
        }

        const commonPayload = {
            id: id,
            name: $('#editUserName').val().trim(),
            userName: $('#editUserUserName').val().trim(),
            mobile: $('#editUserMobile').val().trim(),
            sex: $('#editUserSex').val() === 'true'
        };

        let url = '';
        let payload = null;

        if (currentUserIsRoot) {
            //تست
            console.log('selected user type:', $editUserType.val());
            console.log('options:', $editUserType.find('option').map(function () {
                return {
                    value: this.value,
                    text: this.text
                };
            }).get());
            //تست
            const userType = parseInt($editUserType.val(), 10);

            if (!Object.values(USER_TYPES).includes(userType)) {
                showError('نوع دسترسی انتخاب شده معتبر نیست.');
                return;
            }

            let charityId = null;

            if (userType !== USER_TYPES.ADMIN_SYSTEM) {
                const selectedCharity = $editUserCharity.val();

                if (!selectedCharity) {
                    showError('لطفا خیریه را انتخاب کنید.');
                    return;
                }

                charityId = parseInt(selectedCharity, 10);

                if (Number.isNaN(charityId) || charityId <= 0) {
                    showError('خیریه انتخاب شده معتبر نیست.');
                    return;
                }
            }

            payload = {
                ...commonPayload,
                userType: userType,
                charityId: charityId
            };

            url = `${baseUrl}/api/User/system/${id}`;
        } else {
            payload = commonPayload;
            url = `${baseUrl}/api/User/charity-admin/${id}`;
        }

        $submitBtn.prop('disabled', true).text('در حال ذخیره...');

        $.ajax({
            url: url,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            headers: getAuthHeaders(),
            success: function (response) {
                if (response?.hasError || response?.HasError) {
                    showError(response?.errorMessage || response?.ErrorMessage || 'ویرایش کاربر انجام نشد.');
                    return;
                }

                $editModal.modal('hide');

                Swal.fire({
                    title: 'عملیات موفق!',
                    text: 'اطلاعات کاربر با موفقیت به‌روزرسانی شد.',
                    icon: 'success',
                    customClass: { confirmButton: 'btn btn-success' },
                    buttonsStyling: false
                });

                dataTable.ajax.reload(null, false);
            },
            error: function (xhr) {
                const errorMsg =
                    xhr.responseJSON?.errorMessage ||
                    xhr.responseJSON?.ErrorMessage ||
                    xhr.responseJSON?.message ||
                    'خطایی در سرور رخ داده است';

                showError(errorMsg);
            },
            complete: function () {
                $submitBtn.prop('disabled', false).text('به‌روزرسانی');
            }
        });
    });

    $tableEl.find('tbody').on('click', '.user-delete', function () {
        const id = $(this).data('id');
        const name = $(this).data('name');


        const isSelf = id === currentUserId;

        console.log({
            currentUserId,
            targetUserId: id,
            isSelf
        });

        if (!id) {
            showError('شناسه کاربر معتبر نیست.');
            return;
        }

        const userId = $(this).data('id');


        Swal.fire({
            title: 'آیا مطمئن هستید؟',
            text: `کاربر "${name || ''}" حذف خواهد شد.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'بله، حذف شود',
            cancelButtonText: 'لغو',
            customClass: {
                confirmButton: 'btn btn-danger me-2',
                cancelButton: 'btn btn-label-secondary'
            },
            buttonsStyling: false
        }).then(function (result) {
            if (!result.isConfirmed) return;

            $.ajax({
                url: `${baseUrl}/api/User/${id}`,
                type: 'DELETE',
                headers: getAuthHeaders(),
                success: function (response) {
                    if (response?.hasError || response?.HasError) {
                        showError(response?.errorMessage || response?.ErrorMessage || 'حذف کاربر انجام نشد.');
                        return;
                    }

                    Swal.fire({
                        title: 'عملیات موفق!',
                        text: 'کاربر با موفقیت حذف شد.',
                        icon: 'success',
                        customClass: { confirmButton: 'btn btn-success' },
                        buttonsStyling: false
                    });

                    dataTable.ajax.reload(null, false);
                },
                error: function (xhr) {
                    const errorMsg =
                        xhr.responseJSON?.errorMessage ||
                        xhr.responseJSON?.ErrorMessage ||
                        xhr.responseJSON?.message ||
                        'خطا در حذف کاربر';

                    showError(errorMsg);
                }
            });
        });
    });

  




    $tableEl.find('tbody').on('click', '.user-reset-password', function (e) {

        e.preventDefault();

        const id = $(this).data('id');
        const name = $(this).data('name');
        const isSelf = Number(id) === Number(currentUserId);

        console.log({ targetId: id, currentId: currentUserId, isSelf });

        $('#changePasswordUserId').val(id);
        $('#currentPassword').val('');
        $('#newPassword').val('');
        $('#confirmPassword').val('');

        $('#changePasswordModalLabel').text(`تغییر رمز عبور ${name}`);

        // فقط برای خود کاربر فیلد «رمز فعلی» نمایش داده شود
        $('#currentPasswordContainer').toggleClass('d-none', !isSelf);

        $('#changePasswordError').addClass('d-none').text('');

        const modalElement = document.getElementById('changePasswordModal');
        const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

        modal.show();
    });




    // نمایش/عدم نمایش رمز (چشم)
    $(document).on('click', '.toggle-password', function () {

        const target = $(this).data('target');
        const input = $('#' + target);
        const icon = $(this).find('i');

        if (input.attr('type') === 'password') {
            input.attr('type', 'text');
            icon.removeClass('ti-eye').addClass('ti-eye-off');
        }
        else {
            input.attr('type', 'password');
            icon.removeClass('ti-eye-off').addClass('ti-eye');
        }
    });




    $(document).on('click', '#submitChangePassword', function () {

        const id = Number($('#changePasswordUserId').val());
        const currentPassword = $('#currentPassword').val();
        const newPassword = $('#newPassword').val();
        const confirmPassword = $('#confirmPassword').val();

        const isSelf = id === Number(currentUserId);

        const errorElement = $('#changePasswordError');
        const submitButton = $('#submitChangePassword');
        const loadingElement = $('#submitChangePasswordLoading');

        errorElement
            .addClass('d-none')
            .text('');

        if (isSelf && !currentPassword) {
            errorElement
                .removeClass('d-none')
                .text('رمز عبور فعلی را وارد کنید.');

            return;
        }

        if (!newPassword) {
            errorElement
                .removeClass('d-none')
                .text('رمز عبور جدید را وارد کنید.');

            return;
        }

        if (newPassword !== confirmPassword) {
            errorElement
                .removeClass('d-none')
                .text('تکرار رمز عبور با رمز عبور جدید یکسان نیست.');

            return;
        }

        submitButton.prop('disabled', true);
        loadingElement.removeClass('d-none');

        $.ajax({
            // ۱. اصلاح متغیر userId به id
            // ۲. استفاده از baseUrl تعریف شده در بالای فایل به جای هاردکد کردن پورت
            url: `${baseUrl}/api/User/change-password/${id}`,
            type: 'PUT',
            contentType: 'application/json',
            headers: getAuthHeaders(),

            data: JSON.stringify({
                currentPassword: isSelf ? currentPassword : null,
                newPassword: newPassword,
                confirmPassword: confirmPassword
            }),

            success: function (response) {
                // هماهنگ سازی با ساختار PascalCase یا camelCase خروجی API
                const hasError = response.hasError ?? response.HasError;
                const errorMessage = response.errorMessage ?? response.ErrorMessage;

                if (hasError) {
                    errorElement
                        .removeClass('d-none')
                        .text(errorMessage || 'خطا در تغییر رمز عبور.');

                    return;
                }

                const modalElement = document.getElementById('changePasswordModal');
                const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

                modal.hide();

                Swal.fire({
                    title: 'موفق!',
                    text: 'رمز عبور با موفقیت تغییر کرد',
                    icon: 'success',
                    customClass: { confirmButton: 'btn btn-success' },
                    buttonsStyling: false
                });
            },

            error: function (xhr) {
                errorElement
                    .removeClass('d-none')
                    .text(
                        xhr.responseJSON?.errorMessage ||
                        xhr.responseJSON?.ErrorMessage ||
                        'خطا در تغییر رمز عبور'
                    );
            },

            complete: function () {
                submitButton.prop('disabled', false);
                loadingElement.addClass('d-none');
            }
        });
    });


});
