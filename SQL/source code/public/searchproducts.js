function searchProductsByNames() {
    //get the first name 
    var name_search_string = document.getElementById('product_string').value
    //construct the URL and redirect to it
    window.location = '/products/search/' + encodeURI(name_search_string)
}