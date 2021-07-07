
function filterCustomersByMembership() {
    //get the id of the selected homeworld from the filter dropdown
    var membership_id = document.getElementById('membership_filter').value
    //construct the URL and redirect to it
    window.location = '/customers/filter/' + Number.parseInt(membership_id)
}

function ascCustomersByBirth(){
    window.location = '/customers/asc/'
}

function descCustomersByBirth(){
    window.location = '/customers/desc/'
}