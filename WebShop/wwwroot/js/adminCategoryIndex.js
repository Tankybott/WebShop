var dataTable;

$(document).ready(function () {
    if ($('#categoryFilter').length > 0) {
        loadDataTable();
        $('#CreateSubcategoryInThisCategory').hide();
    } else {
        console.error("Element #categoryFilter not found!");
    }
});

$('#categoryFilter').change(function () {
    var selectedValue = $(this).val();
    menageSubcategoryAddButtonVisibility(selectedValue);
    loadDataTable();
});

function menageSubcategoryAddButtonVisibility(selectedValue) {
    if (selectedValue === "all") {
        $('#CreateSubcategoryInThisCategory').hide();
    } else {
        $('#CreateSubcategoryInThisCategory').show();
        $('#CreateSubcategoryInThisCategory').attr("href", `/Admin/AdminCategory/AddSubcategoryToSpecyficCategory?parentCategoryFilter=${selectedValue}`);
    }
}

function loadDataTable() {
    var selectedParentCategoryId = $('#categoryFilter').val();


    if ($.fn.DataTable.isDataTable('#tblData')) {
        $('#tblData').DataTable().destroy();
    }

    $('#tblData').find('tbody').empty();

    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: `/Admin/AdminCategory/getall?filter=${selectedParentCategoryId}`,
            dataSrc: function (json) {
                console.log("Received JSON:", json); 
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
                "width": "60%"
            },
            {
                data: null,
                "render": function (data) {
                    if (!data || typeof data.id === 'undefined') {
                        console.warn("Invalid data received for rendering:", data);
                        return '';
                    }

                    const categoryId = data.id;
                    let parentCategoryId = data.parentCategoryId || 0;

                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/admincategory/upsert?id=${categoryId}&bindedParentCategory=${parentCategoryId}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i> Edit
                        </a>
                        <a onClick=Delete('/admin/admincategory/delete/?id=${categoryId}') class="btn btn-danger mx-2">
                            <i class="bi bi-trash-fill"></i> Delete
                        </a>
                    </div>`;
                },
                "width": "40%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure? This will delete the category along with all its subcategories.",
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
                    toastr.error("Failed to delete the category.");
                }
            });
        }
    });
}