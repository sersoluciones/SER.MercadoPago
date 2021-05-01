using System;
using System.Collections.Generic;
using System.Net;

namespace MercadoPago
{
    /// <summary>
    /// Class with request options 
    /// </summary>
    public class MPRequestOptions
    {
        public string AccessToken { get; set; }

        public int Timeout { get; set; }

        public int Retries { get; set; }

        public IWebProxy Proxy { get; set; }

        public IDictionary<String, String> CustomHeaders { get; set; }

        public IDictionary<String, String> TrackHeaders { get; private set; }

        public MPRequestOptions(SDK sDK)
        {
            Timeout = sDK.RequestsTimeout;
            Retries = sDK.RequestsRetries;
            Proxy = sDK.Proxy;
            CustomHeaders = new Dictionary<String, String>();
            TrackHeaders = new Dictionary<String, String> {
                { "x-platform-id", sDK.GetPlatformId() },
                { "x-corporation-id", sDK.GetCorporationId() },
                { "x-integrator-id", sDK.GetIntegratorId() }
            };
        }
    }
}