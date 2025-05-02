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
            url: `/Admin/Carrier/GetAll`,
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
                data: 'name',
                "width": "70%"
            },
            {
                data: null,
                "render": function (data) {
                    if (!data || typeof data.id === 'undefined') {
                        console.warn("Invalid data received for rendering:", data);
                        return '';
                    }

                    const carrierId = data.id;

                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/Carrier/upsert?id=${carrierId}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i> Edit
                        </a>
                        <a onClick=DeleteCarrier('/admin/Carrier/delete/?id=${carrierId}') class="btn btn-danger mx-2">
                            <i class="bi bi-trash-fill"></i> Delete
                        </a>
                    </div>`;
                },
                "width": "30%"
            }
        ]
    });
}

window.DeleteCarrier = function (url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data && data.success) {
                        toastr.success("Carrier deleted successfully.");
                        location.reload();
                    } else {
                        toastr.error("An error occurred during deletion.");
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error during delete request:", error);
                    toastr.error("Failed to delete the carrier.");
                }
            });
        }
    });
}