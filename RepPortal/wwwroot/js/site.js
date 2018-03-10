// Write your JavaScript code.

$(document).ready(function () {
   

    $.ajax({
        url: "/Stores/StoresList",
        method: "GET"
    }).then(response => {
        let stores = response
        // parse the lat and long and create an object to pass to google api
        let latLong = {
            "lat": parseFloat(stores[0].lat),
            "lng": parseFloat(stores[0].long)
        }

        
        //create map and center it around the users first store
        let storeMap = new google.maps.Map(document.getElementById('map'), {
            zoom: 15,
            center: latLong
        });

        //creates markers for all of the stores associated with that user
        response.forEach(s => {
            if (s.lat && s.lng) {
                // parse the lat and long and create an object to pass to google api
                const latLong = {
                    "lat": parseFloat(s.lat),
                    "lng": parseFloat(s.long)
                }

                let marker = new google.maps.Marker({
                    position: latLong,
                    //put markers on map created above
                    map: storeMap
                });

            }
        })
    })


});





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

                }

                //submit form
            } else {
                alert("Error retrieving geolocation coordinates. Check your address and try again.")
            }

        })
    }

    $('form').submit()


})