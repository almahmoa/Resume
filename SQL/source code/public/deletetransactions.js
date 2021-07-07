function deleteTransaction(id) {
    $.ajax({
        url: '/transactions/' + id,
        type: 'DELETE',
        success: function (result) {
            window.location.reload(true);
        }
    })
};