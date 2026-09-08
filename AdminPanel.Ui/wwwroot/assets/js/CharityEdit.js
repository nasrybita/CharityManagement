

document.addEventListener("DOMContentLoaded", function () {


    // =========================================================================
    // 1. مدیریت لوگو و بنر
    // =========================================================================

    function setupFilePreview(
        fileInputId,
        previewImgId,
        placeholderId,
        removeInputId,
        editBtnId
    ) {

        const fileInput = document.getElementById(fileInputId);
        const previewImg = document.getElementById(previewImgId);
        const placeholder = document.getElementById(placeholderId);
        const removeInput = document.getElementById(removeInputId);
        const editBtn = document.getElementById(editBtnId);


        if (!fileInput)
            return;



        if (editBtn) {

            editBtn.addEventListener("click", function () {

                fileInput.click();

            });

        }



        fileInput.addEventListener("change", function () {


            if (this.files && this.files[0]) {


                const reader = new FileReader();


                reader.onload = function (e) {


                    if (previewImg) {

                        previewImg.src = e.target.result;
                        previewImg.classList.remove("d-none");

                    }


                    if (placeholder) {

                        placeholder.classList.add("d-none");

                    }



                    if (removeInput) {

                        removeInput.value = "false";

                    }

                };


                reader.readAsDataURL(this.files[0]);

            }


        });

    }



    setupFilePreview(
        "logoFileInput",
        "logoPreviewImg",
        "logoPlaceholder",
        "RemoveLogo",
        "editLogoBtn"
    );


    setupFilePreview(
        "bannerFileInput",
        "bannerPreviewImg",
        "bannerPlaceholder",
        "RemoveBanner",
        "editBannerBtn"
    );





    // حذف لوگو

    const removeLogoBtn =
        document.getElementById("removeLogoBtn");


    if (removeLogoBtn) {


        removeLogoBtn.addEventListener("click", function () {


            document
                .getElementById("logoPreviewImg")
                ?.classList.add("d-none");


            document
                .getElementById("logoPlaceholder")
                ?.classList.remove("d-none");


            const removeInput =
                document.getElementById("RemoveLogo");


            if (removeInput)
                removeInput.value = "true";



            const fileInput =
                document.getElementById("logoFileInput");


            if (fileInput)
                fileInput.value = "";


        });


    }




    // حذف بنر


    const removeBannerBtn =
        document.getElementById("removeBannerBtn");



    if (removeBannerBtn) {


        removeBannerBtn.addEventListener("click", function () {


            document
                .getElementById("bannerPreviewImg")
                ?.classList.add("d-none");



            document
                .getElementById("bannerPlaceholder")
                ?.classList.remove("d-none");



            const removeInput =
                document.getElementById("RemoveBanner");



            if (removeInput)
                removeInput.value = "true";



            const fileInput =
                document.getElementById("bannerFileInput");



            if (fileInput)
                fileInput.value = "";


        });


    }







    // =========================================================================
    // 2. مدیریت دسته بندی ها
    // =========================================================================



    const categorySelect =
        document.getElementById("categorySelect");



    const addCategoryBtn =
        document.getElementById("addCategoryBtn");



    const categoryContainer =
        document.getElementById("currentCategories");



    if (addCategoryBtn && categorySelect && categoryContainer) {



        addCategoryBtn.addEventListener("click", function () {



            const id = categorySelect.value;


            const text =
                categorySelect.options[
                    categorySelect.selectedIndex
                ].text;



            if (!id)
                return;



            // جلوگیری از تکرار

            if (
                document.querySelector(
                    `.category-item[data-category-id="${id}"]`
                )
            )
                return;



            const html = `

            <div class="badge bg-primary d-flex align-items-center gap-2 p-2 category-item"
                 data-category-id="${id}">


                <span>${text}</span>


                <button type="button"
                        class="btn-close btn-close-white remove-current-category">
                </button>



                <input type="hidden"
                       name="Input.CategoryIds"
                       value="${id}" />


            </div>

            `;



            categoryContainer.insertAdjacentHTML(
                "beforeend",
                html
            );



            categorySelect.value = "";


        });




        categoryContainer.addEventListener(
            "click",
            function (e) {


                if (
                    e.target.classList.contains(
                        "remove-current-category"
                    )
                ) {


                    e.target
                        .closest(".category-item")
                        .remove();


                }


            }
        );


    }








    // =========================================================================
    // 3. مدیریت شبکه های اجتماعی
    // =========================================================================


    const socialRows = document.getElementById("socialRows");
    const addSocialBtn = document.getElementById("addSocialBtn");
    const noSocialMessage = document.getElementById("noSocialMessage");

    function updateSocialIndexes() {
        const rows = socialRows.querySelectorAll(".social-row");

        rows.forEach((row, index) => {
            const idInput = row.querySelector(".social-id");
            const select = row.querySelector(".social-type");
            const valueInput = row.querySelector(".social-value");
            const nameInput = row.querySelector(".social-name");

            if (idInput) {
                idInput.name =
                    `Input.SocialMedias[${index}].Id`;
            }

            if (select) {
                select.name =
                    `Input.SocialMedias[${index}].SocialId`;
            }

            if (valueInput) {
                valueInput.name =
                    `Input.SocialMedias[${index}].Value`;
            }

            if (nameInput) {
                nameInput.name =
                    `Input.SocialMedias[${index}].SocialName`;
            }
        });

        if (noSocialMessage) {
            noSocialMessage.classList.toggle(
                "d-none",
                rows.length > 0
            );
        }
    }

    function setSocialName(row) {
        const select = row.querySelector(".social-type");
        const hiddenName = row.querySelector(".social-name");

        if (select && hiddenName && select.selectedIndex >= 0) {
            hiddenName.value =
                select.options[select.selectedIndex].text;
        }
    }

    function getSocialOptionsHtml() {
        return (window.socialTypes || [])
            .map(type => {
                const id = type.id ?? type.Id;
                const name = type.name ?? type.Name;

                return `<option value="${id}">${name}</option>`;
            })
            .join("");
    }

    // تغییر نوع شبکه اجتماعی
    if (socialRows) {
        socialRows.addEventListener("change", function (event) {
            if (event.target.classList.contains("social-type")) {
                const row = event.target.closest(".social-row");

                if (row) {
                    setSocialName(row);
                }
            }
        });
    }

    // افزودن شبکه اجتماعی جدید
    if (addSocialBtn && socialRows) {
        addSocialBtn.addEventListener("click", function () {
            const index =
                socialRows.querySelectorAll(".social-row").length;

            const options = getSocialOptionsHtml();

            const html = `
            <div class="card mb-2 social-row">

                <input type="hidden"
                       class="social-id"
                       name="Input.SocialMedias[${index}].Id"
                       value="" />

                <input type="hidden"
                       class="social-name"
                       name="Input.SocialMedias[${index}].SocialName"
                       value="" />

                <div class="card-body d-flex gap-2 align-items-end">

                    <div class="flex-grow-1">
                        <label class="form-label">
                            شبکه اجتماعی
                        </label>

                        <select class="form-select social-type"
                                name="Input.SocialMedias[${index}].SocialId">
                            ${options}
                        </select>
                    </div>

                    <div class="flex-grow-1">
                        <label class="form-label">
                            آدرس / شناسه
                        </label>

                        <input class="form-control social-value"
                               name="Input.SocialMedias[${index}].Value" />
                    </div>

                    <button type="button"
                            class="btn btn-outline-danger remove-social">
                        حذف
                    </button>

                </div>
            </div>
        `;

            socialRows.insertAdjacentHTML("beforeend", html);

            const newRow = socialRows.lastElementChild;

            setSocialName(newRow);
            updateSocialIndexes();
        });
    }

    // حذف شبکه اجتماعی
    if (socialRows) {
        socialRows.addEventListener("click", function (event) {
            const removeButton =
                event.target.closest(".remove-social");

            if (!removeButton) {
                return;
            }

            const row =
                removeButton.closest(".social-row");

            if (row) {
                row.remove();
                updateSocialIndexes();
            }
        });
    }

    updateSocialIndexes();
});
