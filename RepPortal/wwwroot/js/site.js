
$(document).ready(function () {
    // initilize tooltips
    $('[data-toggle="tooltip"]').tooltip({
        delay: 400,
        animation: true
    }); 

    // check if the user is on the homepage/dashboard and if they are, run code to create a Google Map view
    if (window.location.pathname === "/" || window.location.pathname === "/index.html") {

        $.ajax({
            url: "/Stores/StoresList",
            method: "GET",
            failure: function (response) {
                alert(response.d);
            }
        }).then(response => {
            let stores = response
            let storeMap;
            var markers = [];
            // variable to hold content of info window, so only one window can be open at a time.
            let prev_infowindow

            // Create the search box and link it to the UI element.
            var input = document.getElementById("pac-input");

            function createMap() {

                //// check if there is a valid response from ajax call
                if (stores[0]) {


                        // create map
                        storeMap = new google.maps.Map(document.getElementById('map'));

                        // create a bounds object to tell Google Maps where to set the center of the map
                        bounds = new google.maps.LatLngBounds();
                        
                    // check if the respone contains coordinates
                    if (stores[0].lat) {
                        //create markers for all of the stores associated with the current user
                        stores.forEach(s => {
                            if (s.lat != null) {

                                // parse the lat and long and create a marker
                                const latLong = {
                                    "lat": parseFloat(s.lat),
                                    "lng": parseFloat(s.long)
                                }

                                // create an info window with details for the store
                                let infowindow = new google.maps.InfoWindow({
                                    content: `<div>
                                            <h5>${s.name}</h5>
                                            <div>${s.streetAddress}</div>
                                            <div>${s.cityStateZip}</div>
                                            <div>Last Order: ${new Date(s.lastOrderDate).toLocaleDateString()}</div>
                                            <a href='./Stores/Details/${s.storeId}'>Details</a>                                                                         
                                            </div>`
                                });

                                // create marker for the store
                                let marker = new google.maps.Marker({
                                    position: latLong,
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
                                    // open new detail window
                                    infowindow.open(storeMap, marker);
                                });
                            }
                        })

                        // set map center and zoom levels to contain all the pins
                        storeMap.fitBounds(bounds);
                        storeMap.panToBounds(bounds);

                        // allows users to search different for other items in the map via Google 
                        
                        let searchBox = new google.maps.places.SearchBox(input);

                        storeMap.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

                        // Create the DIV to hold the control and call the CenterControl()
                        // constructor passing in this DIV.
                        let resetControlDiv = document.createElement('div');
                        let resetControl = new ResetControl(resetControlDiv, storeMap);

                        resetControlDiv.index = 1;
                        storeMap.controls[google.maps.ControlPosition.TOP_RIGHT].push(resetControlDiv);



                        // Bias the SearchBox results towards current map's viewport.
                        storeMap.addListener('bounds_changed', function () {
                            searchBox.setBounds(storeMap.getBounds());
                        });


                        // Listen for the event fired when the user selects a prediction and retrieve
                        // more details for that place.
                        searchBox.addListener('places_changed', function () {
                            let places = searchBox.getPlaces();

                            if (places.length == 0) {
                                return;
                            }

                            // Clear out the old markers.
                            markers.forEach(function (marker) {
                                marker.setMap(null);
                            });
                            markers = [];

                            // For each place, get the icon, name and location.
                            let bounds = new google.maps.LatLngBounds();
                            places.forEach(function (place) {
                                if (!place.geometry) {
                                    console.log("Returned place contains no geometry");
                                    return;
                                }
                                let icon = {
                                    url: place.icon,
                                    size: new google.maps.Size(71, 71),
                                    origin: new google.maps.Point(0, 0),
                                    anchor: new google.maps.Point(17, 34),
                                    scaledSize: new google.maps.Size(25, 25)
                                };

                                let infowindow = new google.maps.InfoWindow({
                                    pixelOffset: new google.maps.Size(-22, 0),
                                    content: `<div>
                                    <h5>${place.name}</h5>
                                    <div>${place.formatted_phone_number}</div>
                                    <div>${place.formatted_address}</div> 
                                    <a href='${place.url}' target="_blank">View On Google Maps</a> 
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

                                // add event listener to new marker
                                newMarker.addListener('click', function () {
                                    //if a detail window is open, close it before opening a new one
                                    if (prev_infowindow) {
                                        prev_infowindow.close();
                                    }
                                    //set prev_infowindow as the current detail window open
                                    prev_infowindow = infowindow;

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
                        }); // end of Searchbox event listener
                    }
                }
                // only create listener on map if the map exists. It will not exist if a user (SalesRep) has no stores attached yet. 
                if (storeMap) {
                    // closes infowindow if user clicks anywhere on the map that is not another marker
                    storeMap.addListener('click', function () {
                        if (prev_infowindow) {
                            prev_infowindow.close();
                        }
                    })

                }
            }
            // run createMap function to create a map with pins for all the stores
            createMap();

            // create the legend to explain what the different marker icon represent
            let legendInnerContainer = document.getElementById('legend-innerContainer');
            for (let key in icons) {
                let type = icons[key];
                let name = type.name;
                let icon = type.icon;
                let div = document.createElement('div');
                div.id = "legend-icons";
                div.innerHTML = '<img src="' + icon + '" text-> ' + name;
                legendInnerContainer.appendChild(div);
            }



            /**
              * The ResetControl adds a control to the map that recenters the map on
              * the users stores.
              * This constructor takes the control DIV as an argument.
              * @constructor
            */
            function ResetControl(controlDiv, map) {

                // Set CSS for the control border.
                let controlUI = document.createElement('div');
                controlUI.style.backgroundColor = '#fff';
                controlUI.style.border = '2px solid #fff';
                controlUI.style.borderRadius = '3px';
                controlUI.style.boxShadow = '0 2px 6px rgba(0,0,0,.3)';
                controlUI.style.cursor = 'pointer';
                controlUI.style.marginBottom = '22px';
                controlUI.style.textAlign = 'center';
                controlUI.title = 'Click to reset the map';
                controlDiv.appendChild(controlUI);

                // Set CSS for the control interior.
                let controlText = document.createElement('div');
                controlText.style.color = 'rgb(25,25,25)';
                controlText.style.fontFamily = 'Roboto,Arial,sans-serif';
                controlText.style.fontSize = '15px';
                controlText.style.lineHeight = '33px';
                controlText.style.paddingLeft = '5px';
                controlText.style.paddingRight = '5px';
                controlText.innerHTML = 'Reset Map';
                controlUI.appendChild(controlText);

                // Setup the click event listeners: simply set the map to Chicago.
                controlUI.addEventListener('click', () => {
                    createMap();
                    input.value = "";
                });
            }
        }) // end of .then() from ajax call
    } // end of if statement checking where the user is in the app
});

// Icons to use as Google Map markers
const icons = {
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



