'use strict';

$(async function () {
    console.log(`${apiBaseUrl}`); 
    const token = $('meta[name="access-token"]').attr('content');
    const userRole = $('meta[name="user-role"]').attr('content');

    const statusFilter = $('#campaignStatusFilter');
    const categoryFilter = $('#categoryFilter');
    const charityFilter = $('#charityFilter');

    function getAuthHeaders() {
        const headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }
        return headers;
    }

    function formatDate(dateValue) {
        if (!dateValue) return '-';

        const date = new Date(dateValue);
        if (isNaN(date.getTime())) return '-';

        return date.toLocaleDateString('fa-IR');
    }

    function formatAmount(value) {
        if (value === null || value === undefined || value === '') return '-';
        const num = Number(value);
        if (isNaN(num)) return '-';
        return num.toLocaleString('fa-IR');
    }

    function getCampaignStatusInfo(status) {
        switch (Number(status)) {
            case 1:
                return { text: 'ایجاد شده', className: 'bg-label-secondary' };
            case 2:
                return { text: 'تایید شده', className: 'bg-label-success' };
            case 3:
                return { text: 'رد شده', className: 'bg-label-danger' };
            case 4:
                return { text: 'تعلیق شده', className: 'bg-label-warning' };
            case 5:
                return { text: 'فعال‌سازی مجدد', className: 'bg-label-info' };
            case 6:
                return { text: 'تأمین مالی موفق', className: 'bg-label-primary' };
            case 7:
                return { text: 'تأمین مالی ناموفق', className: 'bg-label-dark' };
            case 8:
                return { text: 'تعلیق توسط سیستم', className: 'bg-label-danger' };
            case 9:
                return { text: 'تعلیق توسط خیریه', className: 'bg-label-warning' };
            default:
                return { text: 'نامشخص', className: 'bg-label-secondary' };
        }
    }

    function getResponseRoot(response) {
        return response?.value || response?.Value || response || {};
    }

    function getResponseItems(response) {
        const root = getResponseRoot(response);
        return root?.items || root?.Items || root?.data || root?.Data || root || [];
    }

    function getRowValue(row, camelKey, pascalKey) {
        return row?.[camelKey] ?? row?.[pascalKey] ?? null;
    }

    function populateSelect($select, items, valueField, textField, defaultText) {
        $select.empty();
        $select.append(`<option value="">${defaultText}</option>`);

        if (!Array.isArray(items) || !items.length) return;

        items.forEach(item => {
            const value =
                item?.[valueField] ??
                item?.[valueField.charAt(0).toUpperCase() + valueField.slice(1)];

            const text =
                item?.[textField] ??
                item?.[textField.charAt(0).toUpperCase() + textField.slice(1)];

            if (value !== undefined && value !== null && text !== undefined && text !== null) {
                $select.append(`<option value="${value}">${text}</option>`);
            }
        });
    }

    async function loadCategories() {
        try {
            const response = await $.ajax({
                url: `${apiBaseUrl}/api/category`,
                type: 'GET',
                headers: getAuthHeaders()
            });

            const items = getResponseItems(response);

            populateSelect(
                categoryFilter,
                items,
                'id',
                'name',
                'همه دسته‌بندی‌ها'
            );
        } catch (error) {
            console.error('خطا در دریافت دسته‌بندی‌ها', error);
            populateSelect(categoryFilter, [], 'id', 'name', 'همه دسته‌بندی‌ها');
        }
    }

    async function loadCharities() {
        try {
            const response = await $.ajax({
                url: `${apiBaseUrl}/api/charity`,
                type: 'GET',
                headers: getAuthHeaders()
            });

            const items = getResponseItems(response);

            populateSelect(
                charityFilter,
                items,
                'id',
                'name',
                'همه خیریه‌ها'
            );
        } catch (error) {
            console.error('خطا در دریافت خیریه‌ها', error);
            populateSelect(charityFilter, [], 'id', 'name', 'همه خیریه‌ها');
        }
    }

    function initializeDataTable() {
        let dt;

        dt = $('.datatables-campaigns').DataTable({
            processing: true,
            serverSide: true,
            searching: false,
            ordering: false,
            pageLength: 10,
            ajax: function (data, callback) {
                const page = Math.floor((data.start || 0) / (data.length || 10)) + 1;
                const pageSize = data.length || 10;

                $.ajax({
                    url: `${apiBaseUrl}/api/campaign`,
                    type: 'GET',
                    data: {
                        page: page,
                        pageSize: pageSize,
                        campaignStatus: statusFilter.val() || null,
                        categoryId: categoryFilter.val() || null,
                        charityId: charityFilter.val() || null
                    },
                    beforeSend: function (xhr) {
                        if (token) {
                            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                        }
                    },
                    success: function (response) {
                        const root = getResponseRoot(response);
                        const items = root?.items || root?.Items || [];
                        const totalCount = root?.totalCount || root?.TotalCount || 0;

                        callback({
                            draw: data.draw,
                            recordsTotal: totalCount,
                            recordsFiltered: totalCount,
                            data: items
                        });
                    },
                    error: function (xhr) {
                        console.error('خطا در دریافت لیست کمپین‌ها', xhr);

                        callback({
                            draw: data.draw,
                            recordsTotal: 0,
                            recordsFiltered: 0,
                            data: []
                        });
                    }
                });
            },
            columns: [
                {
                    data: null,
                    render: function (data, type, row, meta) {
                        const pageInfo = dt.page.info();
                        return (pageInfo.page * pageInfo.length) + meta.row + 1;
                    }
                },
                {
                    data: 'title',
                    render: function (data, type, row) {
                        return data || row.Title || '-';
                    }
                },
                {
                    data: 'charityName',
                    render: function (data, type, row) {
                        return data || row.CharityName || '-';
                    }
                },
                {
                    data: 'startDate',
                    render: function (data, type, row) {
                        return formatDate(data || row.StartDate);
                    }
                },
                {
                    data: 'endDate',
                    render: function (data, type, row) {
                        return formatDate(data || row.EndDate);
                    }
                },
                {
                    data: 'totalAmount',
                    render: function (data, type, row) {
                        return formatAmount(data ?? row.TotalAmount);
                    }
                },
                {
                    data: 'chargedAmount',
                    render: function (data, type, row) {
                        return formatAmount(data ?? row.ChargedAmount);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        const statusVal =
                            row?.campaignStatus ??
                            row?.CampaignStatus ??
                            data?.campaignStatus ??
                            data?.CampaignStatus ??
                            null;

                        const status = getCampaignStatusInfo(statusVal);

                        return `<span class="badge ${status.className}">${status.text}</span>`;
                    }
                },
                {
                    data: 'createdAt',
                    render: function (data, type, row) {
                        return formatDate(data || row.CreatedAt);
                    }
                },
                {
                    data: null,
                    orderable: false,
                    searchable: false,
                    render: function (data, type, row) {
                        const rowId = row.id ?? row.Id;

                        if (userRole === '1') {
                            return `
                                <div class="d-inline-flex align-items-center">
                                    <a href="/Management/CampaignDetails/${rowId}" class="btn btn-sm btn-label-primary px-3 py-1.5 fw-bold">
                                        جزئیات
                                    </a>
                                </div>`;
                        }

                        return `
                            <div class="d-inline-flex align-items-center">
                                <a href="/Management/EditCampaign?id=${rowId}" class="btn btn-sm btn-icon campaign-edit">
                                    <i class="ti ti-pencil text-primary"></i>
                                </a>
                                <div class="dropdown">
                                    <button class="btn btn-sm btn-icon dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                        <i class="ti ti-dots-vertical text-primary"></i>
                                    </button>
                                    <div class="dropdown-menu dropdown-menu-end">
                                        <a class="dropdown-item" href="/Management/CampaignDetails/${rowId}">جزئیات</a>
                                        <a class="dropdown-item text-danger campaign-delete" data-id="${rowId}">حذف</a>
                                    </div>
                                </div>
                            </div>`;
                    }
                }
            ],
            language: {
                paginate: {
                    next: 'بعدی',
                    previous: 'قبلی'
                },
                emptyTable: 'رکوردی یافت نشد',
                processing: 'در حال بارگذاری...',
                lengthMenu: 'نمایش _MENU_ رکورد',
                info: 'نمایش _START_ تا _END_ از _TOTAL_ رکورد',
                infoEmpty: 'رکوردی برای نمایش وجود ندارد',
                zeroRecords: 'رکوردی یافت نشد'
            }
        });

        return dt;
    }

    async function initializePage() {
        await loadCategories();
        await loadCharities();

        const table = initializeDataTable();

        statusFilter.on('change', function () {
            table.ajax.reload(null, true);
        });

        categoryFilter.on('change', function () {
            table.ajax.reload(null, true);
        });

        charityFilter.on('change', function () {
            table.ajax.reload(null, true);
        });

        $('.datatables-campaigns tbody').on('click', '.campaign-delete', function () {
            const id = $(this).data('id');

            Swal.fire({
                title: 'آیا مطمئن هستید؟',
                text: 'این عملیات قابل بازگشت نخواهد بود و کمپین حذف خواهد شد!',
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
                        url: `${apiBaseUrl}/api/campaign/${id}`,
                        type: 'DELETE',
                        headers: getAuthHeaders(),
                        success: function () {
                            table.ajax.reload(null, false);
                            Swal.fire({
                                icon: 'success',
                                title: 'حذف شد!',
                                text: 'کمپین با موفقیت حذف گردید.',
                                confirmButtonText: 'باشه',
                                customClass: {
                                    confirmButton: 'btn btn-success'
                                }
                            });
                        },
                        error: function (xhr) {
                            const errorMsg = xhr.responseJSON?.errorMessage ||
                                'مشکلی در حذف کمپین به وجود آمد یا دسترسی کافی ندارید.';

                            Swal.fire({
                                title: 'خطا!',
                                text: errorMsg,
                                icon: 'error',
                                confirmButtonText: 'باشه',
                                customClass: {
                                    confirmButton: 'btn btn-primary'
                                }
                            });
                        }
                    });
                }
            });
        });
    }

    await initializePage();
});
