using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShipWatch
{
    class Program
    {
        static void Main(string[] args)
        {
            var mmsi = "219347000"; //MS Stavangerfjord, Fjordline.

            var client = new RestClient("https://id.barentswatch.no");
            var request = new RestRequest("connect/token", Method.Post);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", "Din ID");
            request.AddParameter("client_secret", "Din Secret");
            request.AddParameter("scope", "ais");
            request.AddParameter("grant_type", "client_credentials");
            RestResponse response = client.Execute(request);

            JObject json = JObject.Parse(response.Content);
            var token = json["access_token"];


            var client1 = new RestClient("https://live.ais.barentswatch.no");
            var request1 = new RestRequest("live/v1/latest/ais", Method.Get);
            request1.AddHeader("Authorization", "bearer " + token);
            RestResponse response1 = client1.Execute(request1);
            // Console.WriteLine(response1.Content);

            var latitude = "";
            var longitude = "";
            var name = "";
            var destinasjon = "";
            var eta = "";
            var json1 = JsonConvert.DeserializeObject<List<dynamic>>(response1.Content);
            for (int i = 0; i < json1.Count; i++)
            {
                if (json1[i]["mmsi"] == mmsi && json1[i]["type"] == "Position")
                {
                    latitude = json1[i]["latitude"];
                    longitude = json1[i]["longitude"];
                }
                if (json1[i]["mmsi"] == mmsi && json1[i]["type"] == "Staticdata")
                {
                    name = json1[i]["name"];
                    destinasjon = json1[i]["destination"];
                    eta = json1[i]["eta"];
                }
            }
            if (name.Equals(""))
            {
                Console.WriteLine("Ikke mulig å hente data");
                return;
            }
            string[] dato = { eta.Substring(2, 2), eta.Substring(0, 2), eta.Substring(4) };

            Console.WriteLine("Navn: {0}\nLatitude: {1} Longitude: {2}\nSkipet er på vei til {3}.\nForventet ankomst den {4}.{5}. Klokken {6}", name, latitude, longitude, destinasjon, dato[0], dato[1], dato[2]);
            Console.ReadLine();

        }
    }
}
