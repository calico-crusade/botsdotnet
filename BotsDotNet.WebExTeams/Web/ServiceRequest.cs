using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BotsDotNet.WebExTeams.Web
{
    public class ServiceRequest
    {
        private const string HTTP_BASEURI = "https://api.ciscospark.com/v1/";

        public string BaseUri { get; private set; }
        public string Resource { get; private set; }
        public HttpMethod Method { get; private set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
        public Dictionary<string, object> BodyParams { get; set; }
        public AuthInstance AuthInstance { get; private set; }
       
        public ServiceRequest(AuthInstance auth, HttpMethod method, string baseUri, string resource)
        {
            Method = method;
            BaseUri = baseUri;
            Resource = resource;
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["User-Agent"] = UserAgent.Instance.Name,
                ["Spark-User-Agent"] = UserAgent.Instance.Name,
                ["Authorization"] = "Bearer " + auth.AccessToken
            };
            QueryParams = new Dictionary<string, string>();
            BodyParams = new Dictionary<string, object>();
            AuthInstance = auth;
        }

        public ServiceRequest(AuthInstance auth, HttpMethod method, string resource) : this(auth, method, HTTP_BASEURI, resource) { }

        public ServiceRequest(AuthInstance auth, string resource) : this(auth, HttpMethod.GET, resource) { }
        
        public async Task<Response<T>> Execute<T>()
        {
            var request = new RestRequest(Resource, (Method)Method);

            if (Headers != null && Headers.Count > 0)
                foreach (var hdr in Headers)
                    request.AddHeader(hdr.Key, hdr.Value);

            if (QueryParams != null && QueryParams.Count > 0)
                foreach (var qp in QueryParams)
                    request.AddParameter(qp.Key, qp.Value, ParameterType.GetOrPost);

            if (BodyParams != null && BodyParams.Count > 0)
                foreach (var bp in BodyParams)
                    request.AddParameter(bp.Key, bp.Value, ParameterType.GetOrPost);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var client = new RestClient()
            {
                BaseUrl = new Uri(BaseUri)
            };

            var restResp = await client.ExecuteTaskAsync<T>(request);

            return new Response<T>
            {
                Code = (int)restResp.StatusCode,
                Status = restResp.StatusDescription,
                Headers = restResp.Headers.ToDictionary(t => t.Name, t => t.Value),
                Data = restResp.Data
            };
        }

        public static async Task<Response<T>> Post<T>(AuthInstance auth, string resource, Dictionary<string, string> query = null, Dictionary<string, object> body = null)
        {
            return await new ServiceRequest(auth, HttpMethod.POST, resource)
            {
                BodyParams = body,
                QueryParams = query
            }.Execute<T>();
        }

        public static async Task<Response<T>> Get<T>(AuthInstance auth, string resource, Dictionary<string, string> query = null, Dictionary<string, object> body = null)
        {
            return await new ServiceRequest(auth, HttpMethod.GET, resource)
            {
                BodyParams = body,
                QueryParams = query
            }.Execute<T>();
        }
        
        public static async Task<Response<T>> Delete<T>(AuthInstance auth, string resource, Dictionary<string, string> query = null, Dictionary<string, object> body = null)
        {
            return await new ServiceRequest(auth, HttpMethod.DELETE, resource)
            {
                BodyParams = body,
                QueryParams = query
            }.Execute<T>();
        }

        public static async Task<Response<T>> Put<T>(AuthInstance auth, string resource, Dictionary<string, string> query = null, Dictionary<string, object> body = null)
        {
            return await new ServiceRequest(auth, HttpMethod.PUT, resource)
            {
                BodyParams = body,
                QueryParams = query
            }.Execute<T>();
        }
    }
}
