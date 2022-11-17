using log4net;
using Newtonsoft.Json;
using RestSharp;
using SiteCrawlerApp.Entity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SiteCrawlerApp.Controller
{
    public class ReputaApiController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReputaApiController));

        public JsonSearchMessage JsonSearchMessage { get; set; }
        public string Domain { get; set; }
        public string SearchAPI { get; set; }
        public string Origin { get; set; }
        public string Referer { get; set; }

        public string Authorization { get; set; }
        public string Payload { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public ReputaApiController()
        {
            Origin = "https://reputa.vn";
            Referer = "https://reputa.vn";
        }



        public JsonSearchMessage callApiSearch()
        {
            // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            System.Net.ServicePointManager.Expect100Continue = false;

            JsonSearchMessage jsonObj = null;
            // var options = new RestClientOptions("https://apidn.reputa.vn")
            var options = new RestClientOptions(Domain)
            {
                ThrowOnAnyError = true,
                MaxTimeout = 2000
            };
            var client = new RestClient(options);

            // var requestUri = new Uri("/console/news/search", UriKind.Relative);
            var requestUri = new Uri(SearchAPI, UriKind.Relative);
            var request = new RestRequest(requestUri, Method.Post);

            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36");
            // request.AddHeader("Postman-Token", "");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Access-Control-Request-Headers", "authorization,content-type,organization-id");

            // request.AddHeader("Content-Length", "");
            request.AddHeader("Host", "apidn.reputa.vn");
            request.AddHeader("Origin", Origin);
            request.AddHeader("Referer", Referer);
            request.AddHeader("Sec-Fetch-Site", "same-site");


            request.AddHeader("Cookie", "WEBSVR=2");
            request.AddHeader("Authorization", Authorization);
            request.AddParameter("application/json", Payload, ParameterType.RequestBody);

            var response = client.Execute(request);
            log.Info("PAYLOAD sending: \n" + Payload);
            log.Info("PAYLOAD returned: \n" + response.Content);

            jsonObj = JsonConvert.DeserializeObject<JsonSearchMessage>(response.Content);
            return jsonObj;
        }

    }
}
