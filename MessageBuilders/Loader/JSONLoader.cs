namespace MessageBuilders.Loader
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.IO;

    public static class JSONLoader
    {
        public static JObject LoadFile(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    return (JObject)JToken.ReadFrom(reader);
                }
            }
        }

        public static void WriteFile(JObject obj, string path)
        {
            using (StreamWriter file = File.CreateText(path))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(file))
                {
                    jsonWriter.Formatting = Formatting.Indented;
                    obj.WriteTo(jsonWriter);
                }
            }
        }
    }
}