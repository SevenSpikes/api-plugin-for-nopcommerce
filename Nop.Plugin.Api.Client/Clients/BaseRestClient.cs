using System;
using System.Globalization;
using System.Net;
using System.Text;
using Nop.Plugin.Api.Client.Exceptions;
using Nop.Plugin.Api.Client.Security;
using Nop.Plugin.Api.Client.Serialization;
using Polly;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using RestSharp.Serializers;

namespace Nop.Plugin.Api.Client.Clients
{
    public abstract class BaseRestClient
    {
        private readonly NopApiClient _parent;

        private const int DefaultRetryAttempts = 10;

        private const int DefaultDelayBetweenRetryAttemptsInMs = 250;

        private Token _currentToken;

        private IDeserializer _deserializer;
        
        private ISerializer _serializer;


        protected BaseRestClient(NopApiClient parent)
        {
            _parent = parent;
        }

        protected RestRequest GetRequest(string resource, Method method = Method.GET)
        {
            return new RestRequest(resource, method) { RequestFormat = DataFormat.Json };
        }

        protected IRestResponse<T> Execute<T>(
            IRestRequest request,
            bool throwIfError = true,
            int retryAttempts = DefaultRetryAttempts,
            int waitTimeBetweenAttempts = DefaultDelayBetweenRetryAttemptsInMs)
            where T : new()
        {
            InitRequest(request);

            var policy = GetPolicy<IRestResponse<T>>(retryAttempts, waitTimeBetweenAttempts);

            return policy.Execute(
                () =>
                {
                    var response = GetClient().Execute<T>(request);

                    if (throwIfError)
                    {
                        Process(request, response);
                    }

                    return response;
                });
        }

        protected ISerializer Serializer
        {
            get => _serializer ?? (_serializer = new NewtonsoftSerializer());

            set => _serializer = value;
        }

        protected IDeserializer Deserializer
        {
            get => _deserializer ?? (_deserializer = new NewtonsoftSerializer());

            set => _deserializer = value;
        }

        private void Process(IRestRequest request, IRestResponse response)
        {
            if (response.ResponseStatus == ResponseStatus.Error && response.ErrorException != null)
            {
                // This is likely a transport issue such as the underlying connection was closed
                throw new NopApiClientException(
                    $"Request to {request.Method}:{response.ResponseUri} {response.StatusCode} failed with exception",
                    true);
            }

            if (response.ResponseStatus == ResponseStatus.Error)
            {
                throw new NopApiClientException(
                    $"Failed to connect to REST API {request.Resource}, status: {response.StatusCode}",
                    true);
            }

            ThrowErrorIfAny(response);
        }

        protected bool HasError(IRestResponse response)
        {
            if (response?.Request == null)
            {
                return false;
            }

            return response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted
                                                            && response.StatusCode != HttpStatusCode.Created
                                                            && response.StatusCode != HttpStatusCode.Continue
                                                            && response.StatusCode != HttpStatusCode.NoContent;
        }

        protected void ThrowErrorIfAny(IRestResponse response, string message = null)
        {
            if (response?.Request == null)
            {
                throw new NopApiClientException("null response");
            }

            if (HasError(response))
            {
                NopApiClientExceptionDetails exceptionDetails = null;

                if (response.Request.RequestFormat == DataFormat.Json)
                {
                    if (!JsonSerializationUtility.TryDeserialize(response.Content, out exceptionDetails))
                    {
                        if (exceptionDetails == null && response.ErrorException != null)
                        {
                            exceptionDetails = new NopApiClientExceptionDetails
                            {
                                Message = response.ErrorException.Message,
                                ExceptionType = response.ErrorException.GetType().Name,
                                //ExceptionMessage = response.ErrorException.Message,
                                StackTrace = response.ErrorException.StackTrace
                            };
                        }
                    }
                }

                throw new NopApiClientException(
                    response.StatusCode,
                    response.ResponseUri != null ? response.ResponseUri.ToString() : string.Empty,
                    response.Request.Method.ToString(),
                    message,
                    exceptionDetails);
            }

            if (response.ErrorException != null)
            {
                throw new NopApiClientException(
                    response.StatusCode,
                    response.ResponseUri != null ? response.ResponseUri.ToString() : string.Empty,
                    response.Request.Method.ToString(),
                    response.ErrorException);
            }
        }

        private RestClient GetClient()
        {
            var client = new RestClient(_parent.ServerUrl);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;

            // Override with Newtonsoft JSON Handler
            client.AddHandler("application/json", Deserializer);
            client.AddHandler("text/json", Deserializer);
            client.AddHandler("text/x-json", Deserializer);
            client.AddHandler("text/javascript", Deserializer);
            client.AddHandler("*+json", Deserializer);

            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(
                CurrentToken.AccessToken,
                CurrentToken.TokenType);

            return client;
        }

        protected Token CurrentToken => _currentToken ?? (_currentToken = GetToken());

        private Token GetToken()
        {
            var client = new TokenClient(new Uri(_parent.ServerUrl));

            return client.GetToken(_parent.ClientId, _parent.Secret);
        }

        private void InitRequest(IRestRequest request)
        {
            request.OnBeforeDeserialization = resp =>
            {
                // for individual resources when there's an error to make
                // sure that RestException props are populated
                if ((int)resp.StatusCode >= 400)
                {
                    // have to read the bytes so .Content doesn't get populated
                    const string restException = "{{ \"RestException\" : {0} }}";
                    var content = resp.RawBytes.AsString(); //get the response content
                    var newJson = string.Format(restException, content);

                    resp.Content = null;
                    resp.RawBytes = Encoding.UTF8.GetBytes(newJson.ToString(CultureInfo.InvariantCulture));
                }
            };
        }

        private static Policy<T> GetPolicy<T>(int retryAttempts, int waitTimeBetweenAttempts)
        {
            return Policy<T>.Handle<NopApiClientException>(ex => ex.CanRetry).WaitAndRetry(
                retryAttempts,
                attempt => TimeSpan.FromMilliseconds(waitTimeBetweenAttempts));
        }
    }
}


