function deleteCheckoutCart(id) {
    $.ajax({
        url: '/checkout_carts/' + id,
        type: 'DELETE',
        success: function (result) {
            window.location.reload(true);
        }
    })
};