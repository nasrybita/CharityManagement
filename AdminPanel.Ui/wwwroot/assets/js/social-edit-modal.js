'use strict';

$(function () {
    const apiBaseUrl = 'https://localhost:7209';
    const accessToken =
        window.accessToken ||
        $('meta[name="access-token"]').attr("content");
    const $form = $('#editSocialForm');
    const $modal = $('#editSocialModal');
    const $table = $('.datatables-basic');

    $form.on('submit', function (e) {
        e.preventDefault();

        const id = $('#editSocialId').val();
        const name = $('#editSocialName').val().trim();
        const abbreviation = $('#editSocialAbbreviation').val().trim();
        const $submitBtn = $('#updateSocialBtn');

        $submitBtn.prop('disabled', true).text('در حال ذخیره...');

        // Send as a PUT request to /api/social/{id} with the UpdateSocialRequestDto body
        $.ajax({
            url: `${apiBaseUrl}/api/social/${id}`,
            type: 'PUT',
            contentType: 'application/json',


            headers: {
                Authorization: `Bearer ${accessToken}`
            },

            data: JSON.stringify({
                name: name,
                abbreviation: abbreviation
            }),
            success: function () {
                $modal.modal('hide');

                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        title: 'عملیات موفق!',
                        text: 'شبکه اجتماعی با موفقیت به روز رسانی شد.',
                        icon: 'success',
                        customClass: {
                            confirmButton: 'btn btn-success'
                        },
                        buttonsStyling: false
                    });
                }

                if ($.fn.DataTable.isDataTable($table)) {
                    $table.DataTable().ajax.reload(null, false);
                }
            },
            error: function (xhr) {
                let errorMsg =
                    xhr.responseJSON?.errorMessage ||
                    xhr.responseJSON?.message ||
                    'خطایی در سرور رخ داده است';

                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        title: 'خطا!',
                        text: errorMsg,
                        icon: 'error',
                        customClass: {
                            confirmButton: 'btn btn-primary'
                        },
                        buttonsStyling: false
                    });
                } else {
                    console.error(errorMsg);
                }
            },
            complete: function () {
                $submitBtn.prop('disabled', false).text('به‌روزرسانی');
            }
        });
    });
});

// Open the edit modal and populate the fields with current DataTable values
$('.datatables-basic tbody').on('click', '.social-edit', function () {
    const id = $(this).data('id');
    const name = $(this).data('name');
    const abbreviation = $(this).data('abbreviation');

    $('#editSocialId').val(id);
    $('#editSocialName').val(name);
    $('#editSocialAbbreviation').val(abbreviation);

    $('#editSocialModal').modal('show');
});
