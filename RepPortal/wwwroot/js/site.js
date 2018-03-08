// Write your JavaScript code.

$(document).ready(function () {
    initMap();
   
});


function initMap() {
    var uluru = { "lat": 41.3345876, "lng": -73.06000929999999 };
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 15,
        center: uluru
    });
    var marker = new google.maps.Marker({
        position: uluru,
        map: map
    });
}