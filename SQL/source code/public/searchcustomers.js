function searchEmailByName() {
    //get the first name 
    var email_search_string = document.getElementById('email_string').value
    //construct the URL and redirect to it
    window.location = '/customers/search/' + encodeURI(email_search_string)
}