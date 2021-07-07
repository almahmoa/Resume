
function filterStoresByStates() {
    //get the id of the selected homeworld from the filter dropdown
    var state_id = document.getElementById('state_filter').value
    //construct the URL and redirect to it
    window.location = '/stores/filter/' + String(state_id)
}

function ascStatesByName(){
    window.location = '/stores/asc/'
}

function descStatesByName(){
    window.location = '/stores/desc/'
}