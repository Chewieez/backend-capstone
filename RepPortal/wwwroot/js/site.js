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

//get geocode data of the new store upon creation 
$("#CreateStoreBtn").click(evt => {
    evt.preventDefault()
    //ajax request to get location
    const address = $("#formStreetAddress").val()
    const city = $("#formCity").val()
    //const state = $("#formState").val()
    const zip = $("#formZipcode").val()
    $.ajax({
        method: "GET",
        url: `https://maps.googleapis.com/maps/api/geocode/json?address=${address}+${city}+${zip}&key=${googleAPI.key}`
    }).then(res => {
        //geolocation of the address entered
        let geoLocation = res.results["0"].geometry.location

        //assign to position input
        //let geo = JSON.stringify(geoLocation)
        $("#Map-Lat").val(geoLocation.lat)
        $("#Map-Long").val(geoLocation.lng)

        //submit form
        $('form').submit()
    })
})