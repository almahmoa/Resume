function deleteProduct(id) {
    $.ajax({
        url: '/products/' + id,
        type: 'DELETE',
        success: function (result) {
            window.location.reload(true);
        }
    })
};