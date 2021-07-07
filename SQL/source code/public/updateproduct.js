function updateProduct(id) {
    $.ajax({
        url: '/products/' + id,
        type: 'PUT',
        data: $('#update-product').serialize(),
        success: function (result) {
            window.location.replace("./");
        }
    })
};