function searchEmployeesByLastName() {
    //get the first name 
    var last_name_search_string = document.getElementById('last_name_string').value
    //construct the URL and redirect to it
    window.location = '/employees/search/' + encodeURI(last_name_search_string)
}