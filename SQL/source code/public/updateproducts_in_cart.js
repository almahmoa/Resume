function updateProducts_in_cart(id) {
    $.ajax({
        url: '/products_in_carts/' + id,
        type: 'PUT',
        data: $('#update-products_in_cart').serialize(),
        success: function (result) {
            window.location.replace("./");
        }
    })
};