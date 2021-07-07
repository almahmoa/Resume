
function filterTransactionsByStores() {
    //get the id of the selected homeworld from the filter dropdown
    var store_id = document.getElementById('store_filter').value
    //construct the URL and redirect to it
    window.location = '/transactions/filter/' + Number.parseInt(store_id)
}

function ascTransactionsByDate(){
    window.location = '/transactions/asc/'
}

function descTransactionsByDate(){
    window.location = '/transactions/desc/'
}