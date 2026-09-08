'use strict';

//Let all the page get loaded, then execute codes
$(function () {
    const apiBaseUrl = 'https://localhost:7209'; // API Address
    const $form = $('#addCategoryForm');
    const $modal = $('#addCategoryModal');
    const $table = $('.datatables-basic');


    //Helper function to show success or error messages using SweetAlert2
    function showCategoryMessage(icon, message) {

        //Checks if SweetAlert2 library exists
        if (typeof Swal !== 'undefined') {

            //Displays a popup with the specified icon and message
            Swal.fire({
                title: icon === 'success' ? 'عملیات موفق!' : 'خطا!',
                text: message,
                icon: icon,
                customClass: {
                    confirmButton: icon === 'success' ? 'btn btn-success' : 'btn btn-primary'
                },
                buttonsStyling: false
            });
        } else {
            console.error(message);
        }
    }


    //Save category operations
    $form.on('submit', function (e) {

        //Stop default behavior of form (because we don't want the page to get refreshed, we want to send data using AJAX)
        e.preventDefault();

        //Recieving form values
        const categoryName = $('#categoryName').val().trim();

        //If category name is empty, do not proceed
        if (!categoryName) {
            return;
        }

        //Deactivate button while processing the current request and change its text
        const $submitBtn = $('#saveCategoryBtn');
        const originalBtnText = $submitBtn.text();

        //Change button text to indicate that the request is being processed
        $submitBtn.prop('disabled', true).text('در حال ارسال ...');


        //AJAX settings to send data to API
        $.ajax({
            url: `${apiBaseUrl}/api/category`, // POST category api
            type: 'POST',
            contentType: 'application/json',

            beforeSend: function (xhr) {
                if (typeof accessToken !== 'undefined' && accessToken) {
                    xhr.setRequestHeader(
                        'Authorization',
                        'Bearer ' + accessToken
                    );
                }
            },

            data: JSON.stringify({ name: categoryName }), //JSON data that will be sent to server

            //If servers' response was successful, this function gets executed
            success: function () {

                //Close the modal and reset the form
                $modal.modal('hide');
                $form[0].reset();

                // Display a success message using SweetAlert2
                showCategoryMessage('success', 'دسته بندی با موفقیت اضافه شد.');

                // Refresh the DataTable to show the newly added category
                if ($.fn.DataTable.isDataTable($table)) {
                    $table.DataTable().ajax.reload(null, false);
                }
            },

            //Error section in case of error
            error: function (xhr) {
                const errorMsg =
                    xhr.status === 401
                        ? 'توکن ورود معتبر نیست یا ارسال نشده است.'
                        : xhr.status === 403
                            ? 'شما دسترسی افزودن دسته بندی را ندارید.'
                            : xhr.responseJSON?.errorMessage ||
                            xhr.responseJSON?.message ||
                            'خطایی در سرور رخ داده است';

                showCategoryMessage('error', errorMsg);
            },


            //This function will get executed after the AJAX request is completed, regardless of success or error
            //Reactivates the submit button and restores its original text
            complete: function () {
                $submitBtn.prop('disabled', false).text(originalBtnText);
            }
        });
    });
});



