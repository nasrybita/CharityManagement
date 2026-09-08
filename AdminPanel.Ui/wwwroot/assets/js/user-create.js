$(function () {
    const $form = $("#createUserForm");
    const $charitySelect = $("#user-charity");
    const $userTypeSelect = $("#user-type");
    const $charityWrapper = $("#charity-selection-wrapper");

    const USER_TYPES = {
        ADMIN_SYSTEM: 1,
        CHARITY_ADMIN: 2,
        CHARITY_USER: 3
    };

    const baseUrl =
        window.apiBaseUrl ||
        (typeof apiBaseUrl !== "undefined" ? apiBaseUrl : null);

    const accessToken =
        window.accessToken ||
        $('meta[name="access-token"]').attr("content");

    //Read global Razor values safely to prevent ReferenceError when a variable is not rendered.
    const currentUserIsRoot =
        typeof isCurrentUserRoot !== "undefined" && isCurrentUserRoot === true;

    // Read current user's charity id safely for CharityAdmin user creation flow.
    const currentUserCharity = (typeof currentUserCharityId !== 'undefined' && currentUserCharityId !== null)
        ? String(currentUserCharityId)
        : null;

    // Read redirect URL safely and fallback to management users list.
    const usersListRedirectUrl =
        typeof userListUrl !== "undefined" && userListUrl
            ? userListUrl
            : "/Management/Users";

    console.log("user-create.js loaded");
    console.log("isCurrentUserRoot:", currentUserIsRoot);
    console.log("apiBaseUrl:", baseUrl || "NOT_DEFINED");
    console.log("accessToken exists:", !!accessToken);

    function getAuthHeaders() {
        return accessToken
            ? { Authorization: `Bearer ${accessToken}` }
            : {};
    }

    function showError(message) {
        Swal.fire({
            title: "خطا!",
            text: message,
            icon: "error",
            customClass: { confirmButton: "btn btn-primary" }
        });
    }

    // Centralized save button state handling to avoid duplicated enable/disable code.
    function setSaveButtonLoading(isLoading) {
        const $btn = $("#saveUserBtn");

        if (isLoading) {
            $btn.prop("disabled", true).text("در حال ثبت کاربر...");
            return;
        }

        $btn.prop("disabled", false).text("ثبت کاربر");
    }

    // Validate API base URL before every API call.
    function ensureApiBaseUrl() {
        if (baseUrl) {
            return true;
        }

        console.error("apiBaseUrl is not defined");
        showError("آدرس API تنظیم نشده است.");
        return false;
    }

    function initCharitySelect() {
        $charitySelect.select2({
            placeholder: "انتخاب خیریه",
            allowClear: true,
            language: "fa",
            dir: "rtl"
        });
    }

    function toggleCharitySelection(userType) {
        if (userType === USER_TYPES.ADMIN_SYSTEM) {
            $charityWrapper.addClass("d-none");
            $charitySelect.val("").trigger("change");
        } else {
            $charityWrapper.removeClass("d-none");
        }
    }

    function loadUserTypes() {
        if (!ensureApiBaseUrl()) {
            return;
        }

        console.log("loading user types...");

        $.ajax({
            url: `${baseUrl}/api/user/user-types`,
            type: "GET",
            headers: getAuthHeaders(),

            success: function (response) {

                console.log("user types response:", response);

                const items =
                    response?.value ||
                    response?.Value ||
                    response ||
                    [];

                $userTypeSelect.empty();

                $userTypeSelect.append(
                    '<option value="">نوع دسترسی را انتخاب کنید</option>'
                );

                items.forEach(function (item) {

                    const id = item.id ?? item.Id;
                    const title = item.title ?? item.Title;

                    if (id != null && title) {
                        $userTypeSelect.append(
                            new Option(title, id, false, false)
                        );
                    }
                });


                // پیش فرض برای Root
                $userTypeSelect
                    .val(String(USER_TYPES.CHARITY_USER))
                    .trigger("change");
            },

            error: function (xhr) {
                console.error("load user types error:", xhr);
                showError("بارگذاری نوع دسترسی‌ها انجام نشد.");
            }
        });
    }






    function loadCharities() {
        if (!ensureApiBaseUrl()) {
            return;
        }

        $.ajax({
            url: `${baseUrl}/api/Charity`,
            type: "GET",
            data: {
                Page: 1,
                PageSize: 100
            },
            headers: getAuthHeaders(),
            success: function (response) {

                console.log("charities response:", response);

                const items =
                    response?.value?.items ||
                    response?.value?.Items ||
                    [];

                $charitySelect.empty();

                $charitySelect.append(
                    '<option value="">انتخاب خیریه</option>'
                );

                items.forEach(function (item) {

                    const id = item.id ?? item.Id;
                    const name = item.name ?? item.Name;

                    if (id != null && name) {
                        $charitySelect.append(
                            new Option(name, id, false, false)
                        );
                    }
                });

                $charitySelect.trigger("change");
            },
            error: function (xhr) {
                console.error("load charities error:", xhr);
                showError("بارگذاری لیست خیریه‌ها انجام نشد.");
            }
        });
    }

    if (currentUserIsRoot) {
        initCharitySelect();
        loadUserTypes();
        loadCharities();
    } 
   // else {
    //    // Non-root users are CharityAdmin in this page, so hide role/charity selectors from client flow.
    //    $userTypeSelect.val(String(USER_TYPES.CHARITY_USER));
    //    $charityWrapper.addClass("d-none");
    //}

    $userTypeSelect.on("change", function () {
        const userType = parseInt($(this).val(), 10);

        // Ignore empty or invalid user type values to keep initial placeholder stable.
        if (Number.isNaN(userType)) {
            $charityWrapper.addClass("d-none");
            $charitySelect.val("").trigger("change");
            return;
        }

        toggleCharitySelection(userType);
    });

    $form.on("submit", function (e) {
        e.preventDefault();

        if (!ensureApiBaseUrl()) {
            return;
        }

        setSaveButtonLoading(true);

        const userData = {
            name: $("#user-fullname").val(),
            userName: $("#user-username").val(),
            password: $("#user-password").val(),
            mobile: $("#user-mobile").val(),
            userType: USER_TYPES.CHARITY_USER,
            charityId: null
        };

        if (currentUserIsRoot) {
            const selectedUserType = $userTypeSelect.val();

            if (!selectedUserType) {
                showError("لطفا نوع دسترسی را انتخاب کنید.");
                setSaveButtonLoading(false);
                return;
            }

            userData.userType = parseInt(selectedUserType, 10);

            // Reject invalid user type values before sending request to API.
            if (!Object.values(USER_TYPES).includes(userData.userType)) {
                showError("نوع دسترسی انتخاب شده معتبر نیست.");
                setSaveButtonLoading(false);
                return;
            }

            if (userData.userType === USER_TYPES.ADMIN_SYSTEM) {
                // Root/AdminSystem users must not be linked to any charity.
                userData.charityId = null;
            } else {
                const selectedCharity = $charitySelect.val();

                // CharityAdmin and CharityUser require a charity when created by root.
                if (!selectedCharity) {
                    showError("لطفا خیریه را انتخاب کنید.");
                    setSaveButtonLoading(false);
                    return;
                }

                userData.charityId = parseInt(selectedCharity, 10);

                // Reject invalid charity id values before sending request to API.
                if (Number.isNaN(userData.charityId) || userData.charityId <= 0) {
                    showError("خیریه انتخاب شده معتبر نیست.");
                    setSaveButtonLoading(false);
                    return;
                }
            }
        } else {
            // CharityAdmin context:
            // server will force userType=CharityUser and assign charityId from JWT claims
            userData.userType = USER_TYPES.CHARITY_USER;
            userData.charityId = null;
        }

        $.ajax({
            url: `${baseUrl}/api/auth/register`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(userData),
            headers: getAuthHeaders(),
            success: function (response) {
                if (response.hasError || response.HasError) {
                    Swal.fire({
                        title: "خطا!",
                        text:
                            response.errorMessage ||
                            response.ErrorMessage ||
                            "خطا در ثبت اطلاعات کاربر",
                        icon: "error",
                        customClass: { confirmButton: "btn btn-primary" }
                    });
                    return;
                }

                Swal.fire({
                    title: "موفقیت!",
                    text: "کاربر جدید با موفقیت ثبت شد.",
                    icon: "success",
                    customClass: { confirmButton: "btn btn-success" }
                }).then(() => {
                    window.location.href = usersListRedirectUrl;
                });
            },
            error: function (xhr) {
                const msg =
                    xhr.responseJSON?.errorMessage ||
                    xhr.responseJSON?.ErrorMessage ||
                    "خطا در ثبت اطلاعات کاربر";

                Swal.fire({
                    title: "خطا!",
                    text: msg,
                    icon: "error",
                    customClass: { confirmButton: "btn btn-primary" }
                });
            },
            complete: function () {
                setSaveButtonLoading(false);
            }
        });
    });
});
