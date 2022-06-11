var dataTable;

$(document).ready(function () {
    const urlSearchParams = new URLSearchParams(window.location.search);
    const params = Object.fromEntries(urlSearchParams);
    loadDataTable(params.status);
});

function loadDataTable(status) {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": `/Admin/Order/GetAll?status=${status}`
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "userName", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                     <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-warning rounded">Details</a>
                    `;
                },
                "width": "15%"
            },
        ]
    });
}