
function filterProductsByCompany() {
    //get the id of the selected homeworld from the filter dropdown
    var company_name = document.getElementById('company_filter').value
    //construct the URL and redirect to it
    window.location = '/products/filter/' + String(company_name)
}

function ascProductsBySupply(){
    window.location = '/products/asc/'
}

function descProductsBySupply(){
    window.location = '/products/desc/'
}