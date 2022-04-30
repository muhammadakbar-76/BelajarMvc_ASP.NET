var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            {"data": "title", "width": "15%"},
            {"data": "isbn", "width": "15%"},
            {"data": "price", "width": "15%"},
            {"data": "author", "width": "15%"},
            { "data": "category.name", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                     <a href="/Admin/Product/Upsert?Id=${data}" class="btn btn-warning rounded">Edit</a><a href="javascript:;" onclick="return deleteProduct('/Admin/Product/Delete/${data}')" class="btn btn-danger rounded mx-3">Delete</a>
                    `;
                },
                "width": "15%"
            },
        ]
    });
}

function deleteProduct(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message, "Success!");
                    } else {
                        toastr.error(data.message, "Oops!");
                    }
                },
            })
        }
    })
}