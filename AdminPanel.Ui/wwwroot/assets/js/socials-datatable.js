'use strict';

$(function () {

    const apiBaseUrl = 'https://localhost:7209';
    const accessToken =
        window.accessToken ||
        $('meta[name="access-token"]').attr("content");

    const isSystemAdmin =
        $('meta[name="is-system-admin"]').attr('content') === 'true';


    var table = $('.datatables-basic').DataTable({

        processing: true,
        serverSide: false,
        responsive: false,
        autoWidth: false,


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


        ajax: {
            url: "https://localhost:7209/api/social",
            type: "GET",
            headers: {
                Authorization: `Bearer ${accessToken}`
            },
            dataSrc: function (response) {
                return response.value;
            }
        },


        columns: [
            { data: null },
            { data: 'id' },
            { data: 'name' },
            { data: 'abbreviation' },
            { data: 'createdAt' },
            { data: null }
        ],



        columnDefs: [

            // checkbox
            {
                targets: 0,
                orderable: false,
                searchable: false,

                render: function (data, type, row) {

                    return `
                    <div class="form-check d-flex justify-content-center">
                        <input class="form-check-input dt-checkboxes"
                               type="checkbox"
                               value="${row.id}">
                    </div>`;
                }
            },


            // id
            {
                targets: 1,

                render: function (data) {

                    return data != null
                        ? Number(data).toLocaleString('fa-IR')
                        : '';
                }
            },


            // date
            {
                targets: 4,

                render: function (data) {

                    return data
                        ? new Date(data).toLocaleDateString('fa-IR')
                        : '';
                }
            },


            // actions
            {
                targets: 5,
                orderable: false,
                searchable: false,

                render: function (data, type, row) {


                    let actions = `
                        <a class="dropdown-item social-details"
                        href="javascript:void(0);"
                        data-id="${row.id}">
                        جزئیات
                        </a>
                        `;
                    

                    if (isSystemAdmin) {
                        actions += `
                        <a class="dropdown-item social-edit"
                           data-id="${row.id}"
                           data-name="${row.name}"
                           data-abbreviation="${row.abbreviation}">
                            ویرایش
                        </a>


                        <a class="dropdown-item text-danger social-delete"
                           data-id="${row.id}">
                            حذف
                        </a>
                        `;
                    }



                    return `

                    <div class="dropdown">

                        <button class="btn btn-sm btn-icon dropdown-toggle hide-arrow"
                                data-bs-toggle="dropdown">

                            <i class="ti ti-dots-vertical text-primary"></i>

                        </button>


                        <div class="dropdown-menu dropdown-menu-end">

                            ${actions}

                        </div>

                    </div>`;
                }
            }

        ],



        order: [[1, 'desc']],



        dom: '<"card-header flex-column flex-md-row"<"head-label text-center"><"dt-action-buttons text-end pt-3 pt-md-0"B>><"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',



        displayLength: 7,


        lengthMenu: [
            [7, 10, 25, 50, 75, 100],
            ['۷', '۱۰', '۲۵', '۵۰', '۷۵', '۱۰۰']
        ],



        infoCallback: function (settings, start, end, max, total) {

            return `نمایش ${start.toLocaleString('fa-IR')} تا ${end.toLocaleString('fa-IR')} از ${total.toLocaleString('fa-IR')} مورد`;
        },



        buttons: isSystemAdmin
            ? [
                {
                    text: '<i class="ti ti-plus me-1"></i> <span class="d-none d-sm-inline-block">افزودن شبکه اجتماعی</span>',

                    className: 'btn btn-primary',

                    action: function () {

                        $('#addSocialModal').modal('show');

                    }
                }
            ]

            : []

    });



    $('div.head-label')
        .html('<h5 class="card-title mb-0">فهرست شبکه‌های اجتماعی</h5>');





    // Delete
    $('.datatables-basic tbody').on(
        'click',
        '.social-delete',
        function () {

            const id = $(this).data('id');

            Swal.fire({
                title: 'آیا مطمئن هستید؟',
                text: 'این عملیات قابل بازگشت نخواهد بود!',
                icon: 'warning',

                showConfirmButton: true,
                showCancelButton: true,
                showDenyButton: false,

                confirmButtonText: 'بله، حذف شود',
                cancelButtonText: 'انصراف',

                customClass: {
                    confirmButton: 'btn btn-primary me-3',
                    cancelButton: 'btn btn-label-secondary'
                },

                buttonsStyling: false
            }).then(function (result) {

                if (!result.isConfirmed) {
                    return;
                }

                $.ajax({
                    url: `${apiBaseUrl}/api/social/${id}`,
                    type: 'DELETE',

                    headers: {
                        Authorization: `Bearer ${accessToken}`
                    },

                    success: function () {


                        table.ajax.reload(null, false);

                        Swal.fire({
                            icon: 'success',
                            title: 'حذف شد!',
                            text: 'شبکه اجتماعی با موفقیت حذف گردید.',

                            showConfirmButton: true,
                            showCancelButton: false,
                            showDenyButton: false,

                            confirmButtonText: 'باشه',

                            buttonsStyling: false,
                            customClass: {
                                confirmButton: 'btn btn-primary'
                            }
                        });
                    },

                    error: function () {

                        Swal.fire({
                            title: 'خطا!',
                            text: 'مشکلی در حذف شبکه اجتماعی به وجود آمد.',
                            icon: 'error',

                            showConfirmButton: true,
                            showCancelButton: false,
                            showDenyButton: false,

                            confirmButtonText: 'باشه',

                            buttonsStyling: false,
                            customClass: {
                                confirmButton: 'btn btn-danger'
                            }
                        });
                    }
                });
            });
        }
    );









    // Details
    $('.datatables-basic tbody').on('click', '.social-details', function () {
        const id = $(this).data('id');

        $.ajax({
            url: `${apiBaseUrl}/api/social/${id}`,
            type: 'GET',
            headers: {
                Authorization: `Bearer ${accessToken}`
            },
            success: function (response) {
                const social = response.value;

                if (!social) {
                    Swal.fire({
                        title: 'خطا!',
                        text: 'اطلاعات شبکه اجتماعی یافت نشد.',
                        icon: 'error',
                        confirmButtonText: 'باشه'
                    });
                    return;
                }

                $('#detailsId').val(social.id ?? '');
                $('#detailsName').val(social.name ?? '');
                $('#detailsAbbreviation').val(social.abbreviation ?? '');
                $('#detailsCreatedAt').val(
                    social.createdAt
                        ? new Date(social.createdAt).toLocaleString('fa-IR')
                        : ''
                );
                $('#detailsModifiedAt').val(
                    social.modifiedAt
                        ? new Date(social.modifiedAt).toLocaleString('fa-IR')
                        : '—'
                );

                $('#detailsSocialModal').modal('show');
            },
            error: function (xhr) {
                let errorMsg =
                    xhr.responseJSON?.errorMessage ||
                    xhr.responseJSON?.message ||
                    'مشکلی در دریافت جزئیات شبکه اجتماعی به وجود آمد.';

                Swal.fire({
                    title: 'خطا!',
                    text: errorMsg,
                    icon: 'error',
                    confirmButtonText: 'باشه'
                });
            }
        });
    });






});
