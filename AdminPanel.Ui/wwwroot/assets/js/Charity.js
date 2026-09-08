
var fa_translation = {
    lengthMenu: "نمایش _MENU_ ردیف",
    zeroRecords: "رکوردی یافت نشد",
    info: "نمایش _START_ تا _END_ از _TOTAL_ ردیف",
    infoEmpty: "نمایش 0 تا 0 از 0 ردیف",
    infoFiltered: "(فیلتر شده از _MAX_ ردیف)",
    search: "جستجو:",
    paginate: {
        first: "ابتدا",
        last: "انتها",
        next: '<i class="ti ti-chevron-left ti-xs"></i>',
        previous: '<i class="ti ti-chevron-right ti-xs"></i>'
    }
};

$(function () {
    $.fn.dataTable.ext.errMode = 'none';

    var dtAdvFilterTable = $('.dt-advanced-search');
    var dtAdvFilter = null;

    if (!dtAdvFilterTable.length) return;

    dtAdvFilter = dtAdvFilterTable.DataTable({
        dom: '<"row"<"col-sm-12 col-md-6 d-flex justify-content-start"l><"col-sm-12 col-md-6 d-flex justify-content-end"f>><"row"<"col-sm-12"tr>><"row"<"col-sm-12 col-md-6 d-flex justify-content-start"p><"col-sm-12 col-md-6 d-flex justify-content-end"i>>',
        ajax: {
            url: '/Management/Charities?handler=AllCharities',
            type: 'GET',
            dataSrc: function (json) {
                return json.data || [];
            }
        },
        columns: [
            { data: null },
            { data: 'id' },
            { data: 'name' },
            { data: 'telephone' },
            { data: 'website' },
            { data: 'createdAt' },
            { data: null } // Actions Column
        ],
        columnDefs: [
            {
                className: 'control',
                orderable: false,
                targets: 0,
                render: function () { return ''; }
            },
            {
     




                targets: -1,
                title: 'عملیات',
                orderable: false,
                searchable: false,
                render: function (data, type, full) {
                    let actions = `<div class="d-inline-block text-nowrap">
                      <a href="/Management/CharityDetails?id=${full.id}" class="btn btn-sm btn-icon" title="مشاهده">
                          <i class="ti ti-eye"></i>
                      </a>`;

                    // بررسی دسترسی برای نمایش دکمه ویرایش
                    const hasEditAccess = typeof charityPermissions !== 'undefined' && (
                        charityPermissions.canEditAll ||
                        (charityPermissions.canEditOwnOnly && charityPermissions.currentCharityId == full.id)
                    );

                    // اگر کاربر دسترسی ویرایش داشت، دکمه ویرایش نمایش داده شود
                    if (hasEditAccess) {
                        actions += `<a href="/Management/EditCharity?id=${full.id}" class="btn btn-sm btn-icon" title="ویرایش">
                          <i class="ti ti-edit"></i>
                      </a>`;
                    }

                   
                    // دکمه حذف فقط برای ادمین کل سیستم نمایش داده می‌شود
                    // دکمه حذف فقط برای AdminSystem
                    if (typeof charityPermissions !== 'undefined' && charityPermissions.canEditAll) {
                        actions += `
        <button onclick="deleteCharity(${full.id})" 
                class="btn btn-sm btn-icon btn-text-danger" 
                title="حذف">
            <i class="ti ti-trash"></i>
        </button>`;
                    }



                    actions += `</div>`;
                    return actions;
                }

            }
        ],
        language: fa_translation,
        // سایر تنظیمات مثل responsive و غیره بدون تغییر باقی می‌ماند...
    });

    // کدهای فیلتر و جستجو بدون تغییر باقی می‌مانند...


    $(document).on("click", ".btn-edit-charity", function () {

        const id = $(this).data("id");

        window.location.href = `/Management/EditCharity/${id}`;

    });
});
