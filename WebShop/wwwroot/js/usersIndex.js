var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }

    $('#tblData').find('tbody').empty();

    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: "/Admin/Users/GetUsers",
            dataSrc: function (json) {
                if (json && json.data) {
                    return json.data;
                } else {
                    console.error("Unexpected data format:", json);
                    return [];
                }
            },
            error: function (xhr, status, error) {
                console.error("Error during DataTable load:", error);
            }
        },
        "pageLength": 10,
        "lengthChange": false,
        "columns": [
            {
                data: 'email',
                "width": "50%"
            },
            {
                data: 'role',
                "width": "25%"
            },
            {
                data: null,
                render: function (data) {
                    if (data.role === "HeadAdmin") {
                        return ""; // Don't show button for HeadAdmin
                    }

                    const isBanned = data.isUserBanned;
                    const btnClass = isBanned ? 'btn-success' : 'btn-danger';
                    const icon = isBanned ? 'bi-unlock-fill' : 'bi-lock-fill';
                    const label = isBanned ? 'Unban' : 'Ban';

                    return `<button class="btn ${btnClass} toggle-ban-btn" data-email="${data.email}">
                                <i class="bi ${icon}"></i> ${label}
                            </button>`;
                },
                "width": "25%"
            }
        ]
    });

    $('#tblData').off('click', '.toggle-ban-btn').on('click', '.toggle-ban-btn', function () {
        const email = $(this).data('email');
        const $btn = $(this);
        const action = $btn.text().trim().toLowerCase();

        Swal.fire({
            title: `Are you sure you want to ${action} this user?`,
            text: `${email}`,
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: `Yes, ${action}!`
        }).then((result) => {
            if (result.isConfirmed) {
                $btn.prop('disabled', true);

                $.ajax({
                    url: '/Admin/Users/ToggleUserBan',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(email),
                    success: function (data) {
                        if (data.success) {
                            toastr.success(data.message || "Ban status updated.");
                            $('#tblData').DataTable().ajax.reload(null, false);
                        } else {
                            toastr.error(data.message || "Failed to update ban status.");
                        }
                    },
                    error: function (xhr, status, error) {
                        console.error("Error during toggle ban request:", error);
                        toastr.error("An error occurred while toggling ban status.");
                    },
                    complete: function () {
                        $btn.prop('disabled', false);
                    }
                });
            }
        });
    });
}
