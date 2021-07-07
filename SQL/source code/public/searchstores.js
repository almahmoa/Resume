function searchCitiesByName() {
    //get the first name 
    var city_search_string = document.getElementById('city_string').value
    //construct the URL and redirect to it
    window.location = '/stores/search/' + encodeURI(city_search_string)
}