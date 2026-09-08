'use strict';

$(function () {
    const apiBaseUrl = 'https://localhost:7209';
    const $form = $('#editCategoryForm');
    const $modal = $('#editCategoryModal');
    const $table = $('.datatables-basic');

    $form.on('submit', function (e) {
        e.preventDefault();

        const id = Number($('#editCategoryId').val());
        const name = $('#editCategoryName').val().trim();
        const $submitBtn = $('#updateCategoryBtn');

        if (!id || !name) {
            Swal.fire({
                title: 'خطا!',
                text: 'عنوان دسته بندی را وارد کنید.',
                icon: 'error',
                confirmButtonText: 'باشه'
            });

            return;
        }

        const oldText = $submitBtn.text();

        $submitBtn
            .prop('disabled', true)
            .text('در حال ذخیره...');

        $.ajax({
            url: `${apiBaseUrl}/api/category`,
            type: 'PUT',
            contentType: 'application/json',


            beforeSend: function (xhr) {
                if (typeof accessToken !== 'undefined' && accessToken) {
                    xhr.setRequestHeader(
                        'Authorization',
                        'Bearer ' + accessToken
                    );
                }
            },

            data: JSON.stringify({
                id: id,
                name: name
            }),

            success: function () {
                $modal.modal('hide');

                Swal.fire({
                    title: 'عملیات موفق!',
                    text: 'دسته بندی با موفقیت به‌روزرسانی شد.',
                    icon: 'success',
                    confirmButtonText: 'باشه',
                    customClass: {
                        confirmButton: 'btn btn-success'
                    },
                    buttonsStyling: false
                });

                if ($.fn.DataTable.isDataTable($table)) {
                    $table.DataTable().ajax.reload(null, false);
                }
            },

            error: function (xhr) {
                const errorMessage =
                    xhr.status === 401
                        ? 'توکن ورود معتبر نیست یا ارسال نشده است.'
                        : xhr.status === 403
                            ? 'شما دسترسی ویرایش دسته بندی را ندارید.'
                            : xhr.responseJSON?.errorMessage ||
                            xhr.responseJSON?.message ||
                            'خطایی در ویرایش دسته بندی رخ داده است.';

                Swal.fire({
                    title: 'خطا!',
                    text: errorMessage,
                    icon: 'error',
                    confirmButtonText: 'باشه',
                    customClass: {
                        confirmButton: 'btn btn-primary'
                    },
                    buttonsStyling: false
                });
            },

            complete: function () {
                $submitBtn
                    .prop('disabled', false)
                    .text(oldText);
            }
        });
    });

    $('.datatables-basic tbody').on(
        'click',
        '.category-edit',
        function () {
            const id = $(this).data('id');
            const name = $(this).data('name');

            $('#editCategoryId').val(id);
            $('#editCategoryName').val(name);

            $modal.modal('show');
        }
    );
});
