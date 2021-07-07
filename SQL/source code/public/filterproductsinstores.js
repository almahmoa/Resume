
function filterProductsByStores() {
    //get the id of the selected homeworld from the filter dropdown
    var store_id = document.getElementById('products_in_store_filter').value
    //construct the URL and redirect to it
    window.location = '/products_in_stores/filter/' + Number.parseInt(store_id)
}
