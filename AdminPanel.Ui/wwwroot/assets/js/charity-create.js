'use strict';

$(function () {
    const apiBaseUrl = 'https://localhost:7209'; // API Address
    const $form = $('#createCharityForm');




    //A variable for saving socials loaded from server
    let availableSocials = [];

    //Recieve socials predefined by system admin from API
    function loadSocials() {
        $.ajax({
            url: `${apiBaseUrl}/api/social`,
            type: 'GET',
            headers: {
                Authorization: `Bearer ${token}`
            },
            success: function (response) {
                if (response && !response.hasError && response.value) {
                    availableSocials = response.value;
                    addSocialRow();
                }
            },
            error: function (xhr) {
                console.error('خطا در دریافت لیست شبکه‌های اجتماعی');
                console.log('Status:', xhr.status);
                console.log('Response:', xhr.responseText);
            }
        });
    }







    // 1. Enable Select2 for categories with a maximum selection limit of 5

    const token = $('meta[name="access-token"]').attr('content');
    //test
    console.log('Access token:', token);
    console.log('Access token length:', token?.length);
//

    $('#charity-category').select2({
        placeholder: 'انتخاب دسته‌بندی‌ها',
        maximumSelectionLength: 5,
        language: 'fa',
        dir: 'rtl',
        allowClear: true,
        ajax: {
            url: `${apiBaseUrl}/api/category`,
            type: 'GET',
            dataType: 'json',
            delay: 250, // جلوگیری از ارسال درخواست‌های رگباری هنگام تایپ
            headers: {
                Authorization: `Bearer ${token}`
            },
            data: function (params) {
                return {
                    page: params.page || 1,
                    pageSize: 50 // دریافت تعداد کافی از دسته‌بندی‌ها
                    
                };
            },
            processResults: function (response, params) {
                params.page = params.page || 1;

                const items = response?.value?.items || [];
                const totalCount = response?.value?.totalCount || 0;
                const pageSize = response?.value?.pageSize || 50;

                return {
                    results: items.map(item => ({
                        id: item.id,
                        text: item.name
                    })),
                    pagination: {
                        more: (params.page * pageSize) < totalCount
                    }
                };
            },
            cache: true
        }
    });











    //Function for creating and adding new row of social media
    function addSocialRow() {
        // ایجاد گزینه‌های select بر اساس شبکه‌های اجتماعی موجود در دیتابیس
        let optionsHtml = '<option value="">انتخاب شبکه اجتماعی...</option>';
        availableSocials.forEach(social => {
            optionsHtml += `<option value="${social.id}">${social.name} (${social.abbreviation})</option>`;
        });

        const row = `
        <div class="row g-3 social-row mb-2">
            <!-- انتخاب نوع شبکه اجتماعی -->
            <div class="col-md-5">
                <select class="form-select platform-select" required>
                    ${optionsHtml}
                </select>
            </div>

            <!-- لینک یا مقدار شبکه اجتماعی -->
            <div class="col-md-5">
                <input type="text" 
                       class="form-control link-input" 
                       placeholder="آدرس صفحه یا شناسه کاربری (مثلاً ID)" 
                       required />
            </div>

            <!-- دکمه حذف ردیف -->
            <div class="col-md-2">
                <button type="button" class="btn btn-label-danger btn-icon remove-social">
                    <i class="ti ti-trash"></i>
                </button>
            </div>
        </div>`;

        $('#social-container').append(row);
    }






    // Event of adding new row by clicking on button
    $('#add-social-btn').on('click', function () {
        addSocialRow();
    });




    //Event of deleting row
    $(document).on('click', '.remove-social', function () {
        $(this).closest('.social-row').remove();
    });



    
    //Intial calling of social media while loading of the page
    loadSocials();



    // Submit the form using FormData (to handle file uploads)
    $form.on('submit', function (e) {
        e.preventDefault();

        const $btn = $('#saveCharityBtn');
        $btn.prop('disabled', true).text('در حال ارسال...');

        // Create a FormData object
        const formData = new FormData(this);



        const logoFile = $('#charity-logo')[0]?.files[0];
        const bannerFile = $('#charity-banner')[0]?.files[0];

        console.log('Logo file:', logoFile);
        console.log('Banner file:', bannerFile);

        // اطمینان از اینکه فایل‌ها با نام دقیق DTO ارسال می‌شوند
        formData.delete('LogoFile');
        formData.delete('BannerFile');

        if (logoFile) {
            formData.append('LogoFile', logoFile);
        }

        if (bannerFile) {
            formData.append('BannerFile', bannerFile);
        }


        //Preparation of categories
        formData.delete('CategoryIds');
        const selectedCategories = $('#charity-category').val(); // Output is an array of IDs
        if (selectedCategories && selectedCategories.length > 0) {
            selectedCategories.forEach((id, index) => {
                formData.append(`CategoryIds[${index}]`, id);
            });
        }





        $('.social-row').each(function (index) {
            const $platformSelect = $(this).find('.platform-select');

            const socialId = $platformSelect.val();
            const socialName = $platformSelect.find('option:selected').text();
            const linkValue = $(this).find('.link-input').val();

            if (socialId && linkValue) {
                formData.append(`Socials[${index}].SocialId`, socialId);
                formData.append(`Socials[${index}].SocialName`, socialName);
                formData.append(`Socials[${index}].Value`, linkValue);
            }
        });



        const token = $('meta[name="access-token"]').attr('content');




        //test
        console.log('----- FormData -----');

        for (const [key, value] of formData.entries()) {
            console.log(key, value);
        }

        console.log('--------------------');
//test





        $.ajax({
            url: `${apiBaseUrl}/api/charity`,
            type: 'POST',
            data: formData,
            processData: false, // Required for FormData
            contentType: false, // Required for FormData
            headers: {
                Authorization: `Bearer ${token}`
            },


            success: function (response) {


                // Check API logical errors (ApiMessage.HasError)
                if (response.hasError) {
                    Swal.fire({
                        title: 'خطا!',
                        text: response.errorMessage || 'خطا در ثبت اطلاعات',
                        icon: 'error',
                        customClass: { confirmButton: 'btn btn-primary' }
                    });
                    return; // stop success flow
                }



          

                Swal.fire({
                    title: 'موفقیت!',
                    text: 'خیریه جدید با موفقیت ثبت شد.',
                    icon: 'success',
                    customClass: { confirmButton: 'btn btn-success' }
                }).then(() => {

                    window.location.href = charityListUrl;

                });
            },



            //error: function (xhr) {

            //    let msg = xhr.responseJSON?.errorMessage || 'خطا در ثبت اطلاعات';
            //    Swal.fire({
            //        title: 'خطا!',
            //        text: msg,
            //        icon: 'error',
            //        customClass: { confirmButton: 'btn btn-primary' }
            //    });
            //},


            //تست
            error: function (xhr) {
                console.log('----- API ERROR -----');
                console.log('Status:', xhr.status);
                console.log('Response text:', xhr.responseText);
                console.log('Response json:', xhr.responseJSON);
                console.log('---------------------');

                let msg = 'خطا در ثبت اطلاعات';

                if (xhr.responseJSON?.errorMessage) {
                    msg = xhr.responseJSON.errorMessage;
                }
                else if (xhr.responseJSON?.errors) {
                    msg = Object.entries(xhr.responseJSON.errors)
                        .map(([field, errors]) => `${field}: ${errors.join(' - ')}`)
                        .join('\n');
                }
                else if (xhr.responseJSON?.title) {
                    msg = xhr.responseJSON.title;
                }
                else if (xhr.responseText) {
                    msg = xhr.responseText;
                }

                Swal.fire({
                    title: 'خطا!',
                    text: msg,
                    icon: 'error',
                    customClass: { confirmButton: 'btn btn-primary' }
                });
            },
//تست

            complete: function () {
                $btn.prop('disabled', false).text('ثبت خیریه');
            }
        });



    });
});
