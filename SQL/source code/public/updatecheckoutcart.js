function updateCheckoutCart(id) {
    $.ajax({
        url: '/checkout_carts/' + id,
        type: 'PUT',
        data: $('#update-checkout_cart').serialize(),
        success: function(result) {
            window.location.replace("./");
        }
    })
};