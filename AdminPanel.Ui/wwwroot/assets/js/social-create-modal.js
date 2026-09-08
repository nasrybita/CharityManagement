'use strict';

$(function () {

//    const accessToken =
//        window.accessToken ||
//$('meta[name="access-token"]').attr("content");




    const apiBaseUrl = 'https://localhost:7209';
    const $form = $('#addSocialForm');
    const $modal = $('#addSocialModal');
    const $table = $('.datatables-basic');

    function showSocialMessage(icon, message) {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: icon === 'success' ? 'Success!' : 'Error!',
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

    $form.on('submit', function (e) {
        e.preventDefault();

        const socialName = $('#socialName').val().trim();
        const socialAbbreviation = $('#socialAbbreviation').val().trim();

        if (!socialName || !socialAbbreviation) {
            return;
        }

        const $submitBtn = $('#saveSocialBtn');
        const originalBtnText = $submitBtn.text();

        $submitBtn.prop('disabled', true).text('Sending...');


        //تست
        const accessToken =
            window.accessToken ||
            $('meta[name="access-token"]').attr("content");

        console.log("social token:", accessToken);
        //تست





        // Send as CreateSocialRequestDto containing Name and Abbreviation
        $.ajax({
            url: `${apiBaseUrl}/api/social`,
            type: 'POST',
            contentType: 'application/json',
            headers: {
                Authorization: `Bearer ${accessToken}`
            },
            data: JSON.stringify({
                name: socialName,
                abbreviation: socialAbbreviation
            }),
            success: function () {
                $modal.modal('hide');
                $form[0].reset();

                showSocialMessage('success', 'شبکه اجتماعی با موفقیت اضافه شد.');

                if ($.fn.DataTable.isDataTable($table)) {
                    $table.DataTable().ajax.reload(null, false);
                }
            },
            error: function (xhr) {
                const errorMsg =
                    xhr.responseJSON?.errorMessage ||
                    xhr.responseJSON?.message ||
                    'خطایی در سرور رخ داده است';

                showSocialMessage('error', errorMsg);
            },
            complete: function () {
                $submitBtn.prop('disabled', false).text(originalBtnText);
            }
        });
    });
});
