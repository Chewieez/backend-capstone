// Write your JavaScript code.

$(document).ready(function () {
   

    $.ajax({
        url: "/Stores/StoresList",
        method: "GET"
    }).then(response => {
        let stores = response

        // check if the respone contains coordinates
        if (stores[0].lat) {

            // create map
            let storeMap = new google.maps.Map(document.getElementById('map'));

            // create a bounds object to tell Google Maps where to set the center of the map
            bounds = new google.maps.LatLngBounds();

            //create markers for all of the stores associated with the current user
            stores.forEach(s => {

                // parse the lat and long and create a marker
                const latLong = {
                    "lat": parseFloat(s.lat),
                    "lng": parseFloat(s.long)
                }

                let marker = new google.maps.Marker({
                    position: latLong,
                    //put markers on map created above
                    map: storeMap
                });
                
                // add marker to the bounds method to set Google Maps center to include all pins
                const loc = new google.maps.LatLng(marker.position.lat(), marker.position.lng());
                bounds.extend(loc);

            })


            storeMap.fitBounds(bounds);
            storeMap.panToBounds(bounds);

        }
    })

});


            // parse the lat and long and create an object to pass to google api
            //let latLong = {
            //    "lat": parseFloat(stores[0].lat),
            //    "lng": parseFloat(stores[0].long)
            //}



//function initMap() {
//    var uluru = { "lat": 41.3345876, "lng": -73.06000929999999 };
//    var map = new google.maps.Map(document.getElementById('map'), {
//        zoom: 15,
//        center: uluru
//    });
//    var marker = new google.maps.Marker({
//        position: uluru,
//        map: map
//    });
//}

//get geocode data of the new store upon creation 
$("#CreateStoreBtn").click(evt => {
    // get the value out of the address fields
    const address = $("#formStreetAddress").val() || ""
    const city = $("#formCity").val() || ""
    const zip = $("#formZipcode").val() || ""

    // check that all fields are entered
    if (address !== "" && city !== "" && zip !== "") {
        // prevent form from submitting
        evt.preventDefault()

        //ajax request to get geolocation data for address
        $.ajax({
            method: "GET",
            url: `https://maps.googleapis.com/maps/api/geocode/json?address=${address}+${city}+${zip}&key=${googleAPI.key}`
        }).then(res => {
            // check if a single response came back, meaning an address was good.
            if (res.results.length === 1) {
                //geolocation of the address entered
                let geoLocation = res.results["0"].geometry.location
                if (geoLocation) {
                    //assign to form fields
                    $("#Map-Lat").val(geoLocation.lat)
                    $("#Map-Long").val(geoLocation.lng)

                    // submit form
                    $('form').submit()
                }

            } else {
                alert("Error retrieving geolocation coordinates. Check your address and try again.")
            }

        })
    } else {
        // if store location data not present, submit form so asp.net model validaton can catch error and alert user
        $('form').submit()
    }
   
})