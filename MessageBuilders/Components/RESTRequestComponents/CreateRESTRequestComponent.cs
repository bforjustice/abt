namespace MessageBuilders.Components.RESTRequestComponents
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using RestSharp;
    using System;
    using System.Linq;
    using System.Net;

    class CustomJsonCodec : RestSharp.Serializers.ISerializer, RestSharp.Deserializers.IDeserializer
    {
        private static readonly string _contentType = "application/json";
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            // OpenAPI generated types generally hide default constructors.
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    OverrideSpecifiedNames = true
                }
            }
        };

        public CustomJsonCodec()
        {
        }

        public CustomJsonCodec(JsonSerializerSettings serializerSettings)
        {
            _serializerSettings = serializerSettings;
        }

        public string Serialize(object obj)
        {
            var result = JsonConvert.SerializeObject(obj, _serializerSettings);
            return result;
        }

        public T Deserialize<T>(IRestResponse response)
        {
            var result = (T)Deserialize(response, typeof(T));
            return result;
        }

        /// <summary>
        /// Deserialize the JSON string into a proper object.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="type">Object type.</param>
        /// <returns>Object representation of the JSON string.</returns>
        internal object Deserialize(IRestResponse response, Type type)
        {
            var headers = response.Headers;
            if (type == typeof(byte[])) // return byte array
            {
                return response.RawBytes;
            }

            if (type.Name.StartsWith("System.Nullable`1[[System.DateTime")) // return a datetime object
            {
                return DateTime.Parse(response.Content, null, System.Globalization.DateTimeStyles.RoundtripKind);
            }

            if (type == typeof(String) || type.Name.StartsWith("System.Nullable")) // return primitive type
            {
                return Convert.ChangeType(response.Content, type);
            }

            // at this point, it must be a model (json)
            try
            {
                return JsonConvert.DeserializeObject(response.Content, type, _serializerSettings);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }

        public string ContentType
        {
            get { return _contentType; }
            set { throw new InvalidOperationException("Not allowed to set content type."); }
        }
    }

    public class CreateRESTRequestComponent : ComponentBase<object>, IRESTRequestComponentCreator, IBlockComponent<object>
    {
        public override object Result => throw new NotImplementedException();

        public CreateRESTRequestComponent(string key)
        {
            this.myKey = key;
        }

        public HttpWebRequest Do(HttpWebRequest httpWebRequest)
        {
            return (HttpWebRequest)WebRequest.Create(Convert.ToString((this.subComponent.First().Result)));
        }

        public IRestRequest Do(IRestRequest restRequest)
        {
            IRestRequest req = new RestRequest()
            {
                Resource = this.subComponent.First().Result.ToString(),
                JsonSerializer = new CustomJsonCodec(),
            };

            return req;
        }
    }
}