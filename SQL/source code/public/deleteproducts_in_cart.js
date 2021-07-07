function deleteProducts_in_cart(id) {
    $.ajax({
        url: '/products_in_carts/' + id,
        type: 'DELETE',
        success: function (result) {
            window.location.reload(true);
        }
    })
};