'use strict';

$(function () {
    const apiBaseUrl = 'https://localhost:7209';

    $.ajaxSetup({
        beforeSend: function (xhr) {
            if (typeof accessToken !== 'undefined' && accessToken) {
                xhr.setRequestHeader(
                    'Authorization',
                    'Bearer ' + accessToken
                );
            }
        }
    });

    // دکمه "افزودن دسته بندی" — بدون شرط نقش، برای همه کاربران لاگین‌شده
    let tableButtons = [
        {
            text: '<i class="ti ti-plus me-1"></i> <span class="d-none d-sm-inline-block">افزودن دسته بندی</span>',
            className: 'btn btn-primary',
            action: function () {
                $('#addCategoryModal').modal('show');
            }
        }
    ];

    // Convert table to datatable + Datatable settings
    var table = $('.datatables-basic').DataTable({
        processing: true,
        serverSide: false,
        responsive: false,
        autoWidth: false,

        // Persianizing elements of Datatable
        language: {
            search: 'جستجو:',
            lengthMenu: 'نمایش _MENU_ مورد',
            info: 'نمایش _START_ تا _END_ از _TOTAL_ مورد',
            infoEmpty: 'نمایش ۰ تا ۰ از ۰ مورد',
            zeroRecords: 'موردی پیدا نشد',
            emptyTable: 'داده‌ای برای نمایش وجود ندارد',
            paginate: {
                next: 'بعدی',
                previous: 'قبلی'
            }
        },

        // Ajax settings to get data from API
        ajax: {
            url: `${apiBaseUrl}/api/category`,
            type: 'GET',
            data: function (d) {
                d.page = 1;
                d.pageSize = 1000;
            },
            dataSrc: function (json) {
                if (!json || json.hasError || !json.value || !json.value.items)
                    return [];
                return json.value.items;
            },
            error: function (xhr) {
                console.log('Category API error:', xhr.responseText);
            }
        },

        // Order of columns
        columns: [
            { data: null },        // 0: Checkbox
            { data: 'id' },        // 1: Id
            { data: 'name' },      // 2: Name
            { data: 'createdAt' }, // 3: Created At
            { data: null }         // 4: Actions
        ],

        // Implement custom style or behavior for columns
        columnDefs: [
            {
                // Checkbox Column
                targets: 0,
                orderable: false,
                searchable: false,
                render: function (data, type, row) {
                    return `<div class="form-check d-flex justify-content-center">
                                <input class="form-check-input dt-checkboxes" type="checkbox" value="${row.id}">
                            </div>`;
                }
            },
            {
                // Id Column
                targets: 1,
                render: function (data) {
                    return data != null ? Number(data).toLocaleString('fa-IR') : '';
                }
            },
            {
                // CreatedAt Column
                targets: 3,
                render: function (data) {
                    return data ? new Date(data).toLocaleDateString('fa-IR') : '';
                }
            },
            {
                // ستون اقدامات (Actions) — همه دکمه‌ها بدون شرط نقش نمایش داده می‌شوند
                targets: 4,
                orderable: false,
                searchable: false,
                render: function (data, type, row) {
                    let actionsHtml = `<div class="d-inline-flex align-items-center">`;

                    // دکمه ویرایش
                    actionsHtml += `
                        <a href="javascript:;" class="btn btn-sm btn-icon category-edit" data-id="${row.id}" data-name="${row.name}">
                            <i class="ti ti-pencil text-primary"></i>
                        </a>`;

                    // منوی دراپ‌داون جزئیات و حذف
                    actionsHtml += `
                        <div class="dropdown">
                            <button class="btn btn-sm btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                <i class="ti ti-dots-vertical text-primary"></i>
                            </button>
                            <div class="dropdown-menu dropdown-menu-end">
                                <a class="dropdown-item category-details" data-id="${row.id}">جزئیات</a>
                                <a class="dropdown-item text-danger category-delete" data-id="${row.id}">حذف</a>
                            </div>
                        </div>`;

                    actionsHtml += `</div>`;
                    return actionsHtml;
                }
            }
        ],

        order: [[1, 'desc']],
        dom: '<"card-header flex-column flex-md-row"<"head-label text-center"><"dt-action-buttons text-end pt-3 pt-md-0"B>><"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
        displayLength: 7,
        lengthMenu: [[7, 10, 25, 50, 75, 100], ['۷', '۱۰', '۲۵', '۵۰', '۷۵', '۱۰۰']],

        infoCallback: function (settings, start, end, max, total) {
            return `نمایش ${start.toLocaleString('fa-IR')} تا ${end.toLocaleString('fa-IR')} از ${total.toLocaleString('fa-IR')} مورد`;
        },

        buttons: tableButtons
    });

    // Add title to the card
    $('div.head-label').html('<h5 class="card-title mb-0">فهرست دسته بندی ها</h5>');

    // Delete Category Mechanism
    $('.datatables-basic tbody').on('click', '.category-delete', function () {
        const id = $(this).data('id');
        Swal.fire({
            title: 'آیا مطمئن هستید؟',
            text: "این عملیات قابل بازگشت نخواهد بود!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'بله، حذف شود',
            cancelButtonText: 'انصراف',
            customClass: {
                confirmButton: 'btn btn-primary me-3',
                cancelButton: 'btn btn-label-secondary'
            },
            buttonsStyling: false
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    url: `${apiBaseUrl}/api/category/${id}`,
                    type: 'DELETE',
                    success: function () {
                        table.ajax.reload(null, false);
                        Swal.fire({
                            icon: 'success',
                            title: 'حذف شد!',
                            text: 'دسته بندی با موفقیت حذف گردید.',
                            confirmButtonText: 'باشه',
                            customClass: { confirmButton: 'btn btn-success' }
                        });
                    },
                    error: function (xhr) {
                        const message =
                            xhr.status === 401
                                ? 'توکن ورود معتبر نیست یا ارسال نشده است.'
                                : xhr.status === 403
                                    ? 'شما دسترسی حذف را ندارید.'
                                    : 'مشکلی در حذف دسته بندی به وجود آمد.';

                        Swal.fire({
                            title: 'خطا!',
                            text: message,
                            icon: 'error',
                            confirmButtonText: 'باشه',
                            customClass: { confirmButton: 'btn btn-primary' }
                        });
                    }
                });
            }
        });
    });

    // Display Category Details Mechanism
    $('.datatables-basic tbody').on('click', '.category-details', function () {
        const id = $(this).data('id');
        const $btn = $(this);
        const originalText = $btn.html();
        $btn.html('<i class="ti ti-loader-2 ti-spin"></i>');

        $.ajax({
            url: `${apiBaseUrl}/api/category/${id}`,
            type: 'GET',
            success: function (response) {
                if (response && response.value) {
                    const data = response.value;
                    $('#detailsId').val(data.id.toLocaleString('fa-IR'));
                    $('#detailsName').val(data.name);
                    $('#detailsCreatedAt').val(new Date(data.createdAt).toLocaleDateString('fa-IR'));
                    $('#detailsModifiedAt').val(data.modifiedAt ? new Date(data.modifiedAt).toLocaleDateString('fa-IR') : '---');
                    $('#detailsCategoryModal').modal('show');
                }
            },
            error: function (xhr) {
                const message =
                    xhr.status === 401
                        ? 'توکن ورود معتبر نیست یا ارسال نشده است.'
                        : xhr.status === 403
                            ? 'شما دسترسی مشاهده جزئیات را ندارید.'
                            : 'دریافت اطلاعات با خطا مواجه شد.';

                Swal.fire({
                    title: 'خطا!',
                    text: message,
                    icon: 'error',
                    confirmButtonText: 'باشه'
                });
            },
            complete: function () {
                $btn.html(originalText);
            }
        });
    });
});
