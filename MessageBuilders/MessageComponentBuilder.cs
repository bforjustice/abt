namespace MessageBuilders
{
    using MessageBuilders.Interfaces;
    using MessageBuilders.Loader;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public static class MessageComponentBuilder
    {
        public static IRequestCreator CreateHttpRequester(string requestName, string filePath)
        {
            JObject rawComps = JSONLoader.LoadFile(filePath);

            RESTHttpRequester requester = new RESTHttpRequester();

            foreach (JObject item in rawComps[requestName]["Components"].Children())
            {
                requester.CreateComponent(item);
            }

            requester.CreateWorkflow(rawComps[requestName]["Workflow"].Children());

            return requester;
        }

        public static IRequestCreator CreateRequester(string requestName, string filePath)
        {
            JObject rawComps = JSONLoader.LoadFile(filePath);

            RESTRequester requester = new RESTRequester();

            // First. Create Value Type 
            AddBlocks(requester, rawComps, $"$.{requestName}.Components..[?(@.ValueType == 'Value')]");

            // Second. Create Param Type
            AddBlocks(requester, rawComps, $"$.{requestName}.Components..[?(@.ValueType == 'Parameter')]");

            // Third. Create Comopnent Type
            AddBlocks(requester, rawComps, $"$.{requestName}.Components..[?(@.ValueType == 'Component')]");

            // Forth, Create Workflow
            IEnumerable<JToken> workflows = rawComps
                .SelectTokens($"$.{requestName}.Workflow")
                .First()
                .Children();

            requester.CreateWorkflow(workflows);

            return requester;
        }

        private static void AddBlocks(RESTRequester requester, JObject data, string path)
        {
            IEnumerable<JObject> components = data
                .SelectTokens(path)
                .Cast<JObject>();

            foreach (JObject value in components)
            {
                requester.CreateComponent(value);
            }
        }
    }
}