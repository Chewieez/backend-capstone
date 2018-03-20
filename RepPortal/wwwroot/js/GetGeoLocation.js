//get geocode data of the new store upon creation 

$(".CreateStoreBtn").click(evt => {
    // get the value out of the address fields
    const address = $(".formStreetAddress").val() || "";
    const city = $(".formCity").val() || "";
    const zip = $(".formZipcode").val() || "";
    const formLat = $(".Map-Lat").val() || "";
    const formLong = $(".Map-Long").val() || "";
    // Add more required fields to check


    // check that all fields are entered
    if (address !== "" && city !== "" && zip !== "" && formLat === "" && formLong === "") {
        // prevent form from submitting
        evt.preventDefault();

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
                    $(".Map-Lat").val(geoLocation.lat)
                    $(".Map-Long").val(geoLocation.lng)

                    // submit form
                    $('form').submit()
                }
            } else {
                alert("Error retrieving geolocation coordinates. Check your address and try again.")
            }
        })
    } else {
        // if store location data not present, do not submite form so asp.net model validaton can catch error and alert user
        $('form').submit()
    }
})
