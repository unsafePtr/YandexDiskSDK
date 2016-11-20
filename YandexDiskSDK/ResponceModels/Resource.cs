using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK.ResponceModels
{
    public enum ResourceType { Dir, File }

    public class Resource
    {
        [JsonProperty("public_key")]
        public string PublicKey { get; set; }

        [JsonProperty("_embedded")]
        public ResourceList ResourceList { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("custom_properties")]
        public Dictionary<string, string> Properties { get; set; }

        [JsonProperty("public_url")]
        public string Url { get; set; }

        [JsonProperty("origin_path")]
        public string OriginPath { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("md5")]
        public string Md5Hash { get; set; }

        [JsonProperty("type")]
        public ResourceType Type { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }
}
