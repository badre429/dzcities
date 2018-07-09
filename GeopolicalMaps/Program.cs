using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Linq;
namespace GeopolicalMaps
{
    public class AdmnistrativeLocality
    {
        public int Code { get; set; }
        public int Level { get; set; }
        public int CodeParrent { get; set; }
        public string ISO { get; set; }

        public string URL { get; set; }

        public string URL_Name { get; set; }
        public string JsonMap { get; set; }

        public string Name { get; set; }
        public string NameAr { get; set; }
        public string NameBerber { get; set; }
        public double Area { get; set; }
        public int Population { get; set; }
    }
    class Program
    {
        public static List<AdmnistrativeLocality> all = new List<AdmnistrativeLocality>();
        public static string rootDZ = @"http://www.geopoliticalmaps.com/adm/en/dz/algeria/l1/map-of-province-in-algeria";
        static void Main(string[] args)
        {
            List<AdmnistrativeLocality> l1 = AddLevelOne();
            all.AddRange(l1);
            List<AdmnistrativeLocality> l2 = AddLevelTwo(l1);
            all.AddRange(l2);
            List<AdmnistrativeLocality> l3 = AddLevelThree(l2);
            all.AddRange(l3);

            using (var sw = new StreamWriter("all.json"))
            {
                sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(all));
            }
            foreach (var item in all)
            {
                try
                {
                    var jsonPath = JsonFolder(item.ISO);
                    var jsonUrl = @"https://gm-gpm.s3.amazonaws.com/gj/cntry/dz/" + item.ISO.Replace("-", "_") + ".geojson";
                    DownloadFile(jsonUrl, jsonPath);
                }
                catch (System.Exception)
                {
                }
            }
        }
        private static List<AdmnistrativeLocality> AddLevelThree(List<AdmnistrativeLocality> l2)
        {
            List<AdmnistrativeLocality> l3 = new List<AdmnistrativeLocality>();

            var urll3 = @"http://www.geopoliticalmaps.com/adm/en/dz/algeria/l3/map-of-communes-in-";
            foreach (var dayraItems in l2)
            {
                var wilHtmlPath = htmlCode(dayraItems.ISO);

                var wilL2Url = urll3 + dayraItems.URL.Substring(dayraItems.URL.LastIndexOf('/') + 1);
                var i = wilL2Url.IndexOf("-(");
                if (i > 0)
                {
                    wilL2Url = wilL2Url.Substring(0, i);
                }
                DownloadFile(wilL2Url, wilHtmlPath);
                var doc = new HtmlDocument();
                doc.Load(wilHtmlPath);
                var table = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'table-condensed')]");
                var dayraLines = table.Elements("tr").Skip(1).ToArray();

                foreach (var dayraItem in dayraLines)
                {
                    var baladyaColumns = dayraItem.Elements("td").ToArray();
                    if (baladyaColumns.Length < 4)
                    {
                        continue;
                    }
                    var n = new AdmnistrativeLocality();
                    l3.Add(n);
                    //     <table>
                    // <tr>
                    //   <td>
                    //     <input
                    var id = baladyaColumns[0].Element("table").Element("tr").Element("td").Element("input").GetAttributeValue("id", "");
                    n.ISO = id.Substring(3);
                    n.CodeParrent = dayraItems.Code;
                    var w1 = baladyaColumns[1];
                    var a1 = w1.Element("a");
                    n.Level = 3;
                    n.URL = @"http://www.geopoliticalmaps.com/" + a1.GetAttributeValue("href", "");
                    n.URL_Name = a1.InnerText.Trim();
                    n.Name = (n.URL_Name.Replace("Daira", "").Trim());
                    // n.ISO = wilayaColumns[2].InnerText.Trim();
                    // n.Code = int.Parse(n.ISO.Replace("DZ-", ""));
                    n.NameAr = baladyaColumns[3].InnerText.Replace("دائرة", "").Trim();
                    n.NameBerber = baladyaColumns[4].InnerText.Trim();
                    if (int.TryParse(baladyaColumns[5].InnerText.Trim(), out int population))
                    {
                        n.Population = population;
                    }
                    if (double.TryParse(baladyaColumns[6].InnerText.Trim(), out double area)) n.Area = area;

                }

            }

            return l3;
        }
        private static List<AdmnistrativeLocality> AddLevelTwo(List<AdmnistrativeLocality> l1)
        {
            List<AdmnistrativeLocality> l2 = new List<AdmnistrativeLocality>();

            var urll2 = @"http://www.geopoliticalmaps.com/adm/en/dz/algeria/l2/map-of-districts-in-";
            foreach (var wilayaItem in l1)
            {
                var wilHtmlPath = htmlCode(wilayaItem.ISO);

                var wilL2Url = urll2 + wilayaItem.URL.Substring(wilayaItem.URL.LastIndexOf('/') + 1);
                DownloadFile(wilL2Url, wilHtmlPath);
                var doc = new HtmlDocument();
                doc.Load(wilHtmlPath);
                var table = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'table-condensed')]");
                var wilayaLines = table.Elements("tr").Skip(1).ToArray();

                foreach (var dayraItem in wilayaLines)
                {
                    var wilayaColumns = dayraItem.Elements("td").ToArray();
                    if (wilayaColumns.Length < 4)
                    {
                        continue;
                    }
                    var n = new AdmnistrativeLocality();
                    l2.Add(n);
                    //     <table>
                    // <tr>
                    //   <td>
                    //     <input
                    var id = wilayaColumns[0].Element("table").Element("tr").Element("td").Element("input").GetAttributeValue("id", "");
                    n.ISO = id.Substring(3);
                    n.CodeParrent = wilayaItem.Code;
                    var w1 = wilayaColumns[1];
                    var a1 = w1.Element("a");
                    n.Level = 2;
                    n.URL = @"http://www.geopoliticalmaps.com/" + a1.GetAttributeValue("href", "");
                    n.URL_Name = a1.InnerText.Trim();
                    n.Name = (n.URL_Name.Replace("Daira", "").Trim());
                    // n.ISO = wilayaColumns[2].InnerText.Trim();
                    // n.Code = int.Parse(n.ISO.Replace("DZ-", ""));
                    n.NameAr = wilayaColumns[2].InnerText.Replace("دائرة", "").Trim();
                    n.NameBerber = wilayaColumns[3].InnerText.Trim();
                    if (int.TryParse(wilayaColumns[4].InnerText.Trim(), out int population))
                    {
                        n.Population = population;
                    }
                    if (double.TryParse(wilayaColumns[5].InnerText.Trim(), out double area)) n.Area = area;

                }

            }

            return l2;
        }

        private static List<AdmnistrativeLocality> AddLevelOne()
        {
            var dzHtml = htmlCode("dz");
            DownloadFile(rootDZ, dzHtml);
            var doc = new HtmlDocument();
            doc.Load(dzHtml);

            var table = doc.DocumentNode.SelectSingleNode("//*[contains(@class,'table-condensed')]");
            var wilayaLines = table.Elements("tr").Skip(1).ToArray();
            var l1 = new List<AdmnistrativeLocality>();
            foreach (var item in wilayaLines)
            {
                var wilayaColumns = item.Elements("td").ToArray();
                if (wilayaColumns.Length < 4)
                {
                    continue;
                }
                var n = new AdmnistrativeLocality();
                l1.Add(n);
                var w1 = wilayaColumns[1];
                var a1 = w1.Element("a");
                n.Level = 1;
                n.URL = @"http://www.geopoliticalmaps.com/" + a1.GetAttributeValue("href", "");
                n.URL_Name = a1.InnerText.Trim();
                n.Name = (n.URL_Name.Replace("Province", "").Trim());
                n.ISO = wilayaColumns[2].InnerText.Trim();
                n.Code = int.Parse(n.ISO.Replace("DZ-", ""));
                n.NameAr = wilayaColumns[3].InnerText.Trim();
                n.NameBerber = wilayaColumns[4].InnerText.Trim();
                n.Population = int.Parse(wilayaColumns[5].InnerText.Trim());
                n.Area = double.Parse(wilayaColumns[6].InnerText.Trim());
                Console.WriteLine(item);
            }

            return l1;
        }

        public static string htmlCode(string code)
        {
            return "./html/" + code + ".html";
        }
        public static string JsonFolder(string code)
        {
            return "./geo/" + code + ".json";
        }
        public static void DownloadFile(string downloadUrl, string filePath)
        {
            if (File.Exists(filePath))
            {
                var fi = new FileInfo(filePath);
                if (fi.Length > 0)
                    return;
            }
            WebClient client = new WebClient();
            client.DownloadFile(downloadUrl, filePath);
        }
    }
}
