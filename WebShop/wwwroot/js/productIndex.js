var dataTable;

$(document).ready(function () {
    loadDataTable();
});

$('#categoryFilter').change(function () {
    loadDataTable();
});
    
$('#productOptions').change(function () {
    loadDataTable();
});


function loadDataTable() {
    var selectedCategoryId = $('#categoryFilter').val();
    var selectedFilterOption = $('#productOptions').val();

    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }

    $('#tblData').find('tbody').empty();

    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: `/Admin/Product/GetAllForTable?categoryFilter=${selectedCategoryId}&productFilterOption=${selectedFilterOption}`,
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
                "width": "30%"
            },
            {
                data: 'categoryName', 
                "width": "20%"
            },
            {
                data: 'discountId',
                "render": function (data, type, row) {
                    if (type === 'display') {
                        if (data) {
                            return '<div class="d-flex align-items-center justify-content-center text-success fs-4"><i class="bi bi-check-circle-fill"></i></div>';
                        } else {
                            return ''; 
                        }
                    }
                    return data; 
                },
                "width": "10%"
            },
            {
                data: 'isDiscountActive',
                "render": function (data, type, row) {
                    if (type === 'display') {
                        if (data === true) {
                            return '<div class="d-flex align-items-center justify-content-center text-success fs-4"><i class="bi bi-check-circle-fill"></i></div>';
                        } else {
                            return ''; // Return empty if data is null, undefined, or false
                        }
                    }
                    return data; // Return raw data for other types (e.g., sorting)
                },
                "width": "10%"
            },
            {
                data: null,
                "render": function (data) {
                    if (!data || typeof data.id === 'undefined') {
                        console.warn("Invalid data received for rendering:", data);
                        return '';
                    }

                    const productId = data.id;

                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/Product/upsert?id=${productId}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i> Edit
                        </a>
                        <a onClick=DeleteProduct('/admin/Product/delete/?id=${productId}') class="btn btn-danger mx-2">
                            <i class="bi bi-trash-fill"></i> Delete
                        </a>
                    </div>`;
                },
                "width": "30%"
            }
        ]
    });
}

window.DeleteProduct =  function(url) {
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
                        toastr.success(data.message);
                        location.reload(); 
                    } else {
                        toastr.error(data.message || "An error occurred during deletion.");
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error during delete request:", error);
                    toastr.error("Failed to delete the product.");
                }
            });
        }
    });
}