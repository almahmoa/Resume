
function filterProductsByCarts() {
    //get the id of the selected homeworld from the filter dropdown
    var cart_id = document.getElementById('products_in_cart_filter').value
    //construct the URL and redirect to it
    window.location = '/products_in_carts/filter/' + Number.parseInt(cart_id)
}
