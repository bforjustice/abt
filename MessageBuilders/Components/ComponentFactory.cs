namespace MessageBuilders.Components
{
    using MessageBuilders.Components;
    using MessageBuilders.Interfaces;
    using System;
    using MessageBuilders.Components.GeneralComponents;
    using MessageBuilders.Components.RESTRequestComponents;
    using MessageBuilders.Components.Interfaces;
    using MessageBuilders.Components.RestComponents;
    using MessageBuilders.Components.RESTBuilder.OptionModels;
    using RestSharp;

    public static class ComponentFactory
    {
        public static IBlockComponent<object> CreateComponent(string component, string key)
        {
            switch (component)
            {
                case "SHA256Hash":
                    return new SHA256Component(key);

                case "RegisterAuthActionComponent":
                    return new RegisterAuthActionComponent(key);

                case "SHA512Component":
                    return new SHA512Component(key);

                case "HmacSHA512Component":
                    return new HmacSHA512Component(key);

                case "MD5Hash":
                    return new MD5Component(key);

                case "Base64EncodingComponent":
                    return new Base64EncodingComponent(key);

                case "BitEncodingComponent":
                    return new BitEncodingComponent(key);

                case "ByteToStringEncoder":
                    return new ByteToStringEncodingComponent(key);

                case "HexEncodingComponent":
                    return new HexEncodingComponent(key);

                case "CombineParameterComponent":
                    return new CombineParameterComponent(key);

                case "CombineKeyValueParameterComponent":
                    return new CombineKeyValueParameterComponent(key);

                case "CombinParameterByJSONStringComponent":
                    return new CombinParameterByJSONStringComponent(key);

                case "RequestPathCombiner":
                    return new CombineRequestPathComponent(key);

                case "CombineQueryString":
                    return new CombineQueryString(key);

                case "CombineFullPathComponent":
                    return new CombineFullPathComponent(key);

                case "CreateRESTRequestComponent":
                    return new CreateRESTRequestComponent(key);

                case "InsertMethodComponent":
                    return new InsertMethodComponent(key);

                case "InsertHeaderComponent":
                    return new InsertHeaderComponent(key);

                case "InsertContentTypeComponent":
                    return new InsertContentTypeComponent(component, key);

                case "InsertAcceptComponent":
                    return new InsertAcceptComponent(component, key);

                case "CreatePostMessageComponent":
                    return new CreatePostMessageComponent(key);

                case "AddQueryComponent":
                    return new AddQueryComponent(key);

                case "AddEncodedQueryComponent":
                    return new AddEncodedQueryComponent(key);

                case "AddUpperCaseEncodedQueryComponent":
                    return new AddUpperCaseEncodedQueryComponent(key);

                case "ValueToConvertUriEncodedComponent":
                    return new ValueToConvertUriEncodedComponent(key);

                case "GetMethodComponent":
                    return new GetMethodComponent("Method");

                case "PostMethodComponent":
                    return new PostMethodComponent("Method");

                case "DeleteMethodComponent":
                    return new DeleteMethodComponent("Method");

                case "AddDelimeterBetweenParamsComponent":
                    return new AddDelimeterBetweenParamsComponent(key);

                case "UriEncodingComponent":
                    return new UriEncodingComponent(key);

                case "MappedQueryValueComponent":
                    return new MappedQueryValueComponent(key);

                case "RemoveDelimeterComponent":
                    return new RemoveDelimeterComponent(key);

                //case "RemoveDelimeterComponent":
                //    return new RemoveDelimeterComponent(key);

                default:
                    throw new NotImplementedException();
            }
        }

        public static IParamWorker<RequestOptions> CreateWorker(string component, string key)
        {
            switch (component)
            {
                case "AddPathParam":
                    return new AddPathParam(key);
                case "AddHeaderParam":
                    return new AddHeaderParam(key);
                case "AddQueryParam":
                    return new AddQueryParam(key);
                default:
                    throw new NotImplementedException();
            }
        }

        public static IParameterComponent<object> CreateParameterComponent(string key, string paramType)
        {
            switch (key)
            {
                case "SecretKey":
                    return new ParameterComponent("SecretKey", paramType);

                case "APIKey":
                    return new ParameterComponent("APIKey", paramType);

                default:
                    return new ParameterComponent(key, paramType);
            }
        }

        public static IBlockComponent<object> CreateValueComponent(string type, string key, string value)
        {
            switch (type)
            {
                case "UnixTimeSec":
                    return new UnixTimeSec(key, value);

                case "UtcTimeMSec":
                    return new UtcTimeMSec(key, value);

                case "UtcTime":
                    return new UtcTime(key, value);

                default:
                    return new ValueComponent(key, value);
            }
        }
    }
}