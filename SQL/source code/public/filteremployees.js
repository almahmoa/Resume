
function filterEmployeesByStores() {
    //get the id of the selected homeworld from the filter dropdown
    var store_id = document.getElementById('store_filter').value
    //construct the URL and redirect to it
    window.location = '/employees/filter/' + Number.parseInt(store_id)
}

function ascEmployeesByLastName(){
    window.location = '/employees/asc/'
}

function descEmployeesByLastName(){
    window.location = '/employees/desc/'
}