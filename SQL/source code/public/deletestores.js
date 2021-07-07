function deleteStore(id){
    $.ajax({
        url: '/stores/' + id,
        type: 'DELETE',
        success: function(result){
            window.location.reload(true);
        }
    })
};