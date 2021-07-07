function updateProducts_in_store(id) {
    $.ajax({
        url: '/products_in_stores/' + id,
        type: 'PUT',
        data: $('#update-products_in_stores').serialize(),
        success: function (result) {
            window.location.replace("./");
        }
    })
};