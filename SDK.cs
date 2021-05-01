using MercadoPago;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;

namespace MercadoPago
{
    public class SDK
    {
        public const int DEFAULT_REQUESTS_TIMEOUT = 30000;
        public const int DEFAULT_REQUESTS_RETRIES = 3;
        public const string DEFAULT_BASE_URL = "https://api.mercadopago.com";
        public const string PRODUCT_ID = "BC32BHVTRPP001U8NHL0";
        public const string CLIENT_NAME = "MercadoPago-SDK-DotNet";
        public const string DEFAULT_METRICS_SCOPE = "prod";

        private string _clientSecret;
        private string _clientId;
        private string _accessToken;
        private string _appId;
        private string _baseUrl = DEFAULT_BASE_URL;
        private int _requestsTimeout = DEFAULT_REQUESTS_TIMEOUT;
        private int _requestsRetries = DEFAULT_REQUESTS_RETRIES;
        private IWebProxy _proxy;
        private string _userToken;
        private string _refreshToken;
        private readonly string _version;
        private string _corporationId;
        private string _integratorId;
        private string _platformId;
        private string _trackingId;
        private string _metricsScope = DEFAULT_METRICS_SCOPE;

        public SDK()
        {
            _version = new AssemblyName(typeof(SDK).Assembly.FullName).Version.ToString(3);
            _trackingId = String.Format("platform:{0}|{1},type:SDK{2},so;", Environment.Version.Major, Environment.Version, _version);
        }

        /// <summary>  
        /// Property that represent the client secret token.
        /// </summary>
        public string ClientSecret
        {
            get { return _clientSecret; }
            set
            {
                if (!string.IsNullOrEmpty(_clientSecret))
                {
                    throw new MPConfException("clientSecret setting can not be changed");
                }
                _clientSecret = value;
            }
        }

        /// <summary>
        /// Property that represents a client id.
        /// </summary>
        public string ClientId
        {
            get { return _clientId; }
            set
            {
                if (!string.IsNullOrEmpty(_clientId))
                {
                    throw new MPConfException("clientId setting can not be changed");
                }
                _clientId = value;
            }
        }

        /// <summary>
        /// MercadoPago AccessToken.
        /// </summary>
        public string AccessToken
        {
            get { return _accessToken; }
            set
            {
                //if (!string.IsNullOrEmpty(_accessToken))
                //{
                //    throw new MPConfException("accessToken setting can not be changed");
                //}
                _accessToken = value;
            }
        }

        /// <summary>
        /// MercadoPAgo app id.
        /// </summary>
        public string AppId
        {
            get { return _appId; }
            set
            {
                if (!string.IsNullOrEmpty(_appId))
                {
                    throw new MPConfException("appId setting can not be changed");
                }
                _appId = value;
            }
        }

        /// <summary>
        /// Api base URL. Currently https://api.mercadopago.com
        /// </summary>
        public string BaseUrl
        {
            get { return _baseUrl; }
        }

        /// <summary>
        /// Api requests timeout
        /// </summary>
        public int RequestsTimeout
        {
            get { return _requestsTimeout; }
            set { _requestsTimeout = value; }
        }

        /// <summary>
        /// Api requests retries
        /// </summary>
        public int RequestsRetries
        {
            get
            { return _requestsRetries; }
            set { _requestsRetries = value; }
        }

        public IWebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        public string RefreshToken
        {
            get { return _refreshToken; }
            set { _refreshToken = value; }
        }

        /// <summary>Gets the version of the SDK.</summary>
        public string Version
        {
            get { return _version; }
        }

        /// <summary>Gets the product ID.</summary>
        public string ProductId
        {
            get { return PRODUCT_ID; }
        }

        /// <summary>
        /// Gets the client name.
        /// </summary>
        public string ClientName
        {
            get { return CLIENT_NAME; }
        }

        /// <summary>Gets the tracking ID.</summary>
        public string TrackingId
        {
            get { return _trackingId; }
        }

        /// <summary>
        /// Insight metrics scope
        /// </summary>
        public string MetricsScope
        {
            get { return _metricsScope; }
            set { _metricsScope = value; }
        }

        /// <summary>
        /// Dictionary based configuration. Valid configuration keys:
        /// clientSecret, clientId, accessToken, appId
        /// </summary>
        /// <param name="configurationParams"></param>
        public void SetConfiguration(IDictionary<String, String> configurationParams)
        {
            if (configurationParams == null) throw new ArgumentException("Invalid configurationParams parameter");

            configurationParams.TryGetValue("clientSecret", out _clientSecret);
            configurationParams.TryGetValue("clientId", out _clientId);
            configurationParams.TryGetValue("accessToken", out _accessToken);
            configurationParams.TryGetValue("appId", out _appId);

            String requestsTimeoutStr;
            if (configurationParams.TryGetValue("requestsTimeout", out requestsTimeoutStr))
            {
                Int32.TryParse(requestsTimeoutStr, out _requestsTimeout);
            }

            String requestsRetriesStr;
            if (configurationParams.TryGetValue("requestsRetries", out requestsRetriesStr))
            {
                Int32.TryParse(requestsRetriesStr, out _requestsRetries);
            }

            String proxyHostName;
            String proxyPortStr;
            int proxyPort;
            if (configurationParams.TryGetValue("proxyHostName", out proxyHostName)
                && configurationParams.TryGetValue("proxyPort", out proxyPortStr)
                && Int32.TryParse(proxyPortStr, out proxyPort))
            {
                _proxy = new WebProxy(proxyHostName, proxyPort);

                String proxyUsername;
                String proxyPassword;
                if (configurationParams.TryGetValue("proxyUsername", out proxyUsername)
                    && configurationParams.TryGetValue("proxyPassword", out proxyPassword))
                {
                    _proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
            }
        }

        /// <summary>
        /// Initializes the configurations based in a confiiguration object.
        /// </summary>
        /// <param name="config"></param>
        public void SetConfiguration(IConfiguration config)
        {
            if (config == null) throw new ArgumentException("config parameter cannot be null");

            _clientSecret = GetConfigValue(config, "ClientSecret");
            _clientId = GetConfigValue(config, "ClientId");
            _accessToken = GetConfigValue(config, "AccessToken");
            _appId = GetConfigValue(config, "AppId");

            string requestsTimeoutStr = GetConfigValue(config, "RequestsTimeout");
            int.TryParse(requestsTimeoutStr, out _requestsTimeout);

            string requestsRetriesStr = GetConfigValue(config, "RequestsRetries");
            int.TryParse(requestsRetriesStr, out _requestsRetries);

            string proxyHostName = GetConfigValue(config, "ProxyHostName");
            string proxyPortStr = GetConfigValue(config, "ProxyPort");
            if (!string.IsNullOrEmpty(proxyHostName) && int.TryParse(proxyPortStr, out int proxyPort))
            {
                _proxy = new WebProxy(proxyHostName, proxyPort);

                string proxyUsername = GetConfigValue(config, "ProxyUsername");
                string proxyPassword = GetConfigValue(config, "ProxyPassword");
                if (!string.IsNullOrEmpty(proxyUsername) && !string.IsNullOrEmpty(proxyPassword))
                {
                    _proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                }
            }
        }

        /// <summary>
        /// Clean all the configuration variables
        /// (FOR TESTING PURPOSES ONLY)
        /// </summary>
        public void CleanConfiguration()
        {
            _clientSecret = null;
            _clientId = null;
            _accessToken = null;
            _appId = null;
            _baseUrl = DEFAULT_BASE_URL;
            _requestsTimeout = DEFAULT_REQUESTS_TIMEOUT;
            _requestsRetries = DEFAULT_REQUESTS_RETRIES;
            _proxy = null;
        }

        /// <summary>
        /// Changes base Url
        /// (FOR TESTING PURPOSES ONLY)
        /// </summary>
        public void SetBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        private string GetConfigValue(IConfiguration config, string key)
        {
            string value = null;
            var keyValue = config[key];
            if (keyValue != null)
            {
                value = keyValue;
            }
            return value;
        }

        /// <summary>
        /// Get the access token pointing to OAuth.
        /// </summary>
        /// <returns>A valid access token.</returns>
        public string GetAccessToken()
        {
            if (String.IsNullOrEmpty(AccessToken))
            {
                AccessToken = MPCredentials.GetAccessToken(this);
            }
            return AccessToken;
        }

        /// <summary>
        /// Sets the access token.
        /// </summary>
        /// <param name="accessToken">Value of the access token.</param>
        public void SetAccessToken(string accessToken)
        {
            //if (!string.IsNullOrEmpty(AccessToken))
            //{
            //    throw new MPException("Access_Token setting cannot be changed.");
            //}

            AccessToken = accessToken;
        }

        /// <summary>
        /// Gets the custom user token.
        /// This method is deprecated and will be removed in a future version
        /// </summary>
        /// <returns>User token to return.</returns>
        // TODO; remove this method in a future major version
        [Obsolete("There is no use for this method.")]
        public string GetUserToken()
        {
            return _userToken;
        }

        public void SetUserToken(string value)
        {
            _userToken = value;
        }

        public JToken Get(String uri)
        {
            MPRESTClient client = new MPRESTClient(this);
            return client.ExecuteGenericRequest(HttpMethod.GET, uri, PayloadType.JSON, null);
        }

        public JToken Post(string uri, JObject payload)
        {
            MPRESTClient client = new MPRESTClient(this);
            return client.ExecuteGenericRequest(HttpMethod.POST, uri, PayloadType.JSON, payload);
        }

        public JToken Put(string uri, JObject payload)
        {
            MPRESTClient client = new MPRESTClient(this);
            return client.ExecuteGenericRequest(HttpMethod.PUT, uri, PayloadType.JSON, payload);
        }

        /// <summary>  
        ///  Property that represent the corporation id.
        /// </summary>
        public string CorporationId
        {
            get { return _corporationId; }
            set
            {
                if (!string.IsNullOrEmpty(_corporationId))
                {
                    throw new MPConfException("corporationId setting can not be changed");
                }
                _corporationId = value;
            }
        }

        /// <summary>  
        ///  Property that represent the integrator id.
        /// </summary>
        public string IntegratorId
        {
            get { return _integratorId; }
            set
            {
                if (!string.IsNullOrEmpty(_integratorId))
                {
                    throw new MPConfException("integratorId setting can not be changed");
                }
                _integratorId = value;
            }
        }

        /// <summary>  
        ///  Property that represent the plataform id.
        /// </summary>
        public string PlatformId
        {
            get { return _platformId; }
            set
            {
                if (!string.IsNullOrEmpty(_platformId))
                {
                    throw new MPConfException("platformId setting can not be changed");
                }
                _platformId = value;
            }
        }

        public void SetCorporationId(string corporationId)
        {
            CorporationId = corporationId;
        }

        public string GetCorporationId()
        {
            return CorporationId;
        }

        public void SetIntegratorId(string integratorId)
        {
            IntegratorId = integratorId;
        }

        public string GetIntegratorId()
        {
            return IntegratorId;
        }

        public void SetPlatformId(string platformId)
        {
            PlatformId = platformId;
        }

        public string GetPlatformId()
        {
            return PlatformId;
        }
    }
}
