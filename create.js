var fs = require("fs");

var contents = fs.readFileSync("orgall.json", "utf8");
var all = [
  {
    id: "1",
    post_code: "1",
    name: "c",
    wilaya_id: "1",
    ar_name: "c",
    longitude: "1",
    latitude: "1"
  }
];

all = JSON.parse(contents);
var outAll = [];
for (let index = 1; index < 49; index++) {
  const fitredCities = all.filter(z => z.wilaya_id == index);
  var output = [];
  fitredCities.forEach(item => {
    output.push({
      code: +item.post_code,
      w: +item.wilaya_id,
      ar: item.ar_name,
      name: item.name,
      lng: +item.longitude,
      lat: +item.latitude
    });
  });
  outAll = [...outAll, ...output];
  fs.writeFileSync("wilaya/" + index + ".json", JSON.stringify(output), "utf8");
}
fs.writeFileSync("all.json", JSON.stringify(outAll), "utf8");

all = JSON.parse(fs.readFileSync("orgwilayas.json", "utf8"));
outAll = all.map(item => ({
  code: +item.id,
  ar: item.ar_name,
  name: item.name,
  lng: +item.longitude,
  lat: +item.latitude
}));
fs.writeFileSync("wilayas.json", JSON.stringify(outAll), "utf8");
