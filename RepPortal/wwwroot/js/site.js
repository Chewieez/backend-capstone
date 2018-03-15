// Write your JavaScript code.

$(document).ready(function () {
    
    if (window.location.pathname === "/" || window.location.pathname === "/index.html") {
    
        $.ajax({
            url: "/Stores/StoresList",
            method: "GET"
        }).then(response => {
            let stores = response
            let storeMap;
            var markers = [];
            // variable to hold content of info window, so only one window can be open at a time.
            let prev_infowindow

            function createMap() {

                // check if there is a valid response from ajax call
                if (stores[0]) {

                    // check if the respone contains coordinates
                    if (stores[0].lat) {

                        // create map
                        storeMap = new google.maps.Map(document.getElementById('map'));

                        // create a bounds object to tell Google Maps where to set the center of the map
                        bounds = new google.maps.LatLngBounds();

                        //var iconBase = 'https://maps.google.com/mapfiles/kml/shapes/';
                        var icons = {
                            1: {
                                name: 'Active',
                                icon: '/images/map-icons/Shopping_Bag_1.svg'
                            },
                            2: {
                                name: 'No Orders > 6 months',
                                icon: '/images/map-icons/Shopping_Bag_2.svg'
                            },
                            3: {
                                name: 'No Orders > 12 months',
                                icon: '/images/map-icons/Shopping_Bag_3.svg'
                            },
                            4: {
                                name: 'Closed',
                                icon: '/images/map-icons/Shopping_Bag_4.svg'
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

                            // add event listener to markers to open info window on click
                            marker.addListener('click', function () {
                                //if a detail window is open, close it before opening a new one
                                if (prev_infowindow) {
                                    prev_infowindow.close();
                                }
                                //set prev_infowindow as the current detail window open
                                prev_infowindow = infowindow;

                                infowindow.open(storeMap, marker);
                            });

                            storeMap.addListener('click', function () {
                                if (prev_infowindow) {
                                    prev_infowindow.close();
                                }
                            })
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
                        
                    }
                }
            }
            createMap();

            //allows users to search different for other items in the map via Google 
            // Create the search box and link it to the UI element.
            var input = document.getElementById('pac-input');
            var searchBox = new google.maps.places.SearchBox(input);
            storeMap.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

            // Bias the SearchBox results towards current map's viewport.
            storeMap.addListener('bounds_changed', function () {
                searchBox.setBounds(storeMap.getBounds());
            });

            
            // Listen for the event fired when the user selects a prediction and retrieve
            // more details for that place.
            searchBox.addListener('places_changed', function () {
                var places = searchBox.getPlaces();

                if (places.length == 0) {
                    return;
                }

                // Clear out the old markers.
                markers.forEach(function (marker) {
                    marker.setMap(null);
                });
                markers = [];

                // For each place, get the icon, name and location.
                var bounds = new google.maps.LatLngBounds();
                places.forEach(function (place) {
                    if (!place.geometry) {
                        console.log("Returned place contains no geometry");
                        return;
                    }
                    var icon = {
                        url: place.icon,
                        size: new google.maps.Size(71, 71),
                        origin: new google.maps.Point(0, 0),
                        anchor: new google.maps.Point(17, 34),
                        scaledSize: new google.maps.Size(25, 25)
                    };

                    var infowindow = new google.maps.InfoWindow({
                        content: `<div>
                                        <h5>${place.name}</h5>
                                        
                                        <div>${place.formatted_address}</div>
                                        
                                                                         
                                        </div>`
                    });

                    // Create a marker for each place.
                    let newMarker = new google.maps.Marker({
                        map: storeMap,
                        icon: icon,
                        title: place.name,
                        position: place.geometry.location
                    })
                    markers.push(newMarker);

                    newMarker.addListener('click', function () {
                        infowindow.open(storeMap, newMarker);
                    });


                    if (place.geometry.viewport) {
                        // Only geocodes have viewport.
                        bounds.union(place.geometry.viewport);
                    } else {
                        bounds.extend(place.geometry.location);
                    }
                });
                storeMap.fitBounds(bounds);
            });


                        //storeMap.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(legend);

        })

    }
});


//get geocode data of the new store upon creation 
$(".CreateStoreBtn").click(evt => {
    // get the value out of the address fields
    const address = $("#formStreetAddress").val() || ""
    const city = $("#formCity").val() || ""
    const zip = $("#formZipcode").val() || ""
    // Add more required fields to check


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