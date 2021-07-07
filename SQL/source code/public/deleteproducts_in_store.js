function deleteProducts_in_store(id) {
    $.ajax({
        url: '/products_in_stores/' + id,
        type: 'DELETE',
        success: function (result) {
            window.location.reload(true);
        }
    })
};