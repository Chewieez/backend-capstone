// Write your JavaScript code.

$(document).ready(function () {
    
    if (window.location.pathname === "/" || window.location.pathname === "/index.html") {
    
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

                //var iconBase = 'https://maps.google.com/mapfiles/kml/shapes/';
                var icons = {
                    1: {
                        name: 'Active',
                        icon: '/images/map-icons/Shopping_Bag_6.svg'
                    },
                    2: {
                        name: 'No Orders > 6 months',
                        icon: '/images/map-icons/Shopping_Bag_7.svg'
                    },
                    3: {
                        name: 'No Orders > 12 months',
                        icon: '/images/map-icons/Shopping_Bag_5.svg'
                    },
                    4: {
                        name: 'Closed',
                        icon: '/images/map-icons/Shopping_Bag_2.svg'
                    }
                }
                //create markers for all of the stores associated with the current user
                stores.forEach(s => {

                    // parse the lat and long and create a marker
                    const latLong = {
                        "lat": parseFloat(s.lat),
                        "lng": parseFloat(s.long)
                    }

                    var infowindow = new google.maps.InfoWindow({
                        content: `<div>
                                <h5>${s.name}</h5>
                                <div>${s.streetAddress}</div>
                                <div>${s.city}, ${s.state.name}</div>
                                <div>${s.zipcode}</div>
                                <a href='./Stores/Details/${s.storeId}'>Details</a>                                     
                                </div>`
                    });

                    let marker = new google.maps.Marker({
                        position: latLong,
                        animation: google.maps.Animation.DROP,
                        title: s.name,
                        icon: {
                            url: icons[s.statusId].icon,
                            scaledSize: new google.maps.Size(64, 64)
                        },
                        map: storeMap
                    });
                
                    // add marker to the bounds method to set Google Maps center to include all pins
                    const loc = new google.maps.LatLng(marker.position.lat(), marker.position.lng());
                    bounds.extend(loc);

                    marker.addListener('click', function () {
                        infowindow.open(storeMap, marker);
                    });
                })


                storeMap.fitBounds(bounds);
                storeMap.panToBounds(bounds);

                var legendInnerContainer = document.getElementById('legend-innerContainer');
                for (var key in icons) {
                    var type = icons[key];
                    var name = type.name;
                    var icon = type.icon;
                    var div = document.createElement('div');
                    div.id = "legend-icons";
                    div.innerHTML = '<img src="' + icon + '"> ' + name;
                    legendInnerContainer.appendChild(div);
                }

                //storeMap.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(legend);

            }
        })

    }
});

// Sets a timeout delay on dropping pins onto the map
//function drop() {
//    for (var i = 0; i < markerArray.length; i++) {
//        setTimeout(function () {
//            addMarkerMethod();
//        }, i * 200);
//    }
//}

//get geocode data of the new store upon creation 
$(".CreateStoreBtn").click(evt => {
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