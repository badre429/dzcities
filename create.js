var fs = require("fs");
const colors = [
  "#e03c28",
  "#d7d7d7",
  "#7b7b7b",
  "#343434",
  "#71a6a1",
  "#bdffca",
  "#25e2cd",
  "#0a98ac",
  "#20b562",
  "#58d332",
  "#139d08",
  "#376d03",
  "#6ab417",
  "#8cd612",
  "#beeb71",
  "#eeffa9",
  "#b6c121",
  "#939717",
  "#cc8f15",
  "#ffbb31",
  "#ffe737",
  "#f68f37",
  "#ad4e1a",
  "#231712",
  "#5c3c0d",
  "#ae6c37",
  "#c59782",
  "#e2d7b5",
  "#4f1507",
  "#823c3d",
  "#da655e",
  "#e18289",
  "#f5b784",
  "#ffe9c5",
  "#ff82ce",
  "#cf3c71",
  "#871646",
  "#a328b3",
  "#cc69e4",
  "#d59cfc",
  "#fec9ed",
  "#e2c9ff",
  "#a675fe",
  "#6a31ca",
  "#5a1991",
  "#211640",
  "#3d34a5",
  "#6264dc",
  "#9ba0ef"
];
function colorFrom(w) {
  return colors[w - 1];
}
function geoPointFromCity(s, marker = "city", markersize = "small") {
  return {
    type: "Feature",
    properties: {
      "marker-size": markersize,
      "marker-symbol": marker,
      "marker-color": colorFrom(s.w||s.code),
      name: s.name,
      ar: s.ar,
      code: s.code,
      w: s.w||s.code
    },
    geometry: {
      type: "Point",
      coordinates: [s.lat, s.lng]
    }
  };
}
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
const allWilayas = all.map(item => ({
  code: +item.id,
  ar: item.ar_name,
  name: item.name,
  lng: +item.longitude,
  lat: +item.latitude
}));
fs.writeFileSync("wilayas.json", JSON.stringify(allWilayas), "utf8");

fs.writeFileSync(
  "geojson.json",
  JSON.stringify({
    type: "FeatureCollection",
    features: [
      ...outAll.map(z => geoPointFromCity(z)),
      ...allWilayas.map(z => geoPointFromCity(z, "star", "medium"))
    ]
  }),
  "utf8"
);
