let dataTable;

$(document).ready(function () {
    loadDataTable();

    $('input[name="orderStatus"]').change(function () {
        loadDataTable();
    });
});

function loadDataTable() {
    if ($.fn.DataTable.isDataTable('#tblOrders')) {
        $('#tblOrders').DataTable().destroy();
    }

    $('#tblOrders').find('tbody').empty();

    dataTable = $('#tblOrders').DataTable({
        "ajax": {
            url: `/User/Order/GetOrders`,
            dataSrc: function (json) {
                if (!json || !json.dtos) {
                    console.error("Unexpected data format:", json);
                    return [];
                }

                const selectedStatus = $('input[name="orderStatus"]:checked').val();

                if (selectedStatus === "All") {
                    return json.dtos;
                }

                const statusMap = {
                    "AwaitingPayment": "created",
                    "PaymentConfirmed": "payment-confirmed", 
                    "Processing": "processing",
                    "Shipped": "shipped"
                };

                const mappedStatus = statusMap[selectedStatus];
                return json.dtos.filter(order => order.orderStatus === mappedStatus);
            },
            error: function (xhr, status, error) {
                console.error("Error loading orders table:", error);
            }
        },
        "pageLength": 10,
        "lengthChange": false,
        "columns": [
            { data: 'id', "width": "15%" },
            { data: 'fullName', "width": "25%" },
            {
                data: 'creationDate',
                "render": function (data) {
                    if (!data || data === "0001-01-01T00:00:00") {
                        return "Not Set";
                    }
                    const date = new Date(data);
                    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
                },
                "width": "25%"
            },
            {
                data: 'orderStatus',
                "render": function (status) {
                    const friendlyNames = {
                        "": "Payment Confirmed",
                        "created": "Awaiting Payment",
                        "payment-confirmed": "Payment Confirmed",
                        "processing": "Processing",
                        "shipped": "Shipped"
                    };
                    return friendlyNames[status] || "Unknown";
                },
                "width": "15%"
            },
            {
                data: null,
                "render": function (data) {
                    if (!data || typeof data.id === 'undefined') {
                        console.warn("Invalid data for actions:", data);
                        return '';
                    }

                    return `<div class="text-center">
                                <a href="/user/Order/Details/${data.id}" class="btn btn-primary btn-sm">
                                    Details
                                </a>
                            </div>`;
                },
                "width": "20%"
            }
        ]
    });
}
