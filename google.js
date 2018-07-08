const https = require("https");
const fs = require("fs");
const fetch = require("node-fetch");

var wilayas = JSON.parse(fs.readFileSync('dzcities/wilayas.json', 'utf8'));
var all = JSON.parse(fs.readFileSync('dzcities/all.json', 'utf8'));
var vil = {};
wilayas.forEach(element => {
    vil[element.code] = element.name;

});
// all = [all[0]];

wilayas.forEach(element => {
    vil[element.code] = element.name;


});

function sleep(ms) {
    return new Promise(resolve => {
        setTimeout(resolve, ms)
    })
}
wilayas = vil;
const getLocation = async (url, code) => {
    try {
        const response = await fetch(url);
        const json = await response.json();
        fs.writeFileSync("dzcities/geo/" + code + ".json", JSON.stringify(json), "utf8");
    } catch (error) {
        console.log(error);
    }
};
var i = 0;
var nextAll = [];
all.forEach(element => {
    var path = "dzcities/geo/" + element.code + ".json";
    if (fs.existsSync(path)) {

        var data = JSON.parse(fs.readFileSync(path, 'utf8'));
        if (data.error_message == null) {
            return;
        }
    }
    nextAll.push(element);

});
all = nextAll;

function downloadElement(element) {

    var wil = wilayas[element.w];
    var path = "dzcities/geo/" + element.code + ".json";

    const url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + element.name + "," + wil + "&language=fr&type=administrative_area_level_3&country=dz&key=AIzaSyCP0TwT4GqTkgi7A-_AOyvF2FkTVZHcwZE";
    console.log(url);
    getLocation(url, element.code);
}

function timeOutDownload() {
    for (let index = 0; index < 10; index++) {
        if (i >= all.length) return;
        const element = all[i];
        i++;
        downloadElement(element);

    }
    if (i < all.length) setTimeout(() => {
        timeOutDownload();
    }, 4000);
}
// all.forEach(async element => {
//     downloadElement(element);



// });

timeOutDownload();