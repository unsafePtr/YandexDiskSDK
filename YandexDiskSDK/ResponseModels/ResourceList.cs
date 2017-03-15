using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.RequestModels;

namespace YandexDiskSDK.ResponseModels
{
    public class ResourceList : IEnumerable<Resource>
    {
        [JsonProperty("sort")]
        [DefaultValue(null)]
        public SortBy? SortBy { get; set; }

        [JsonProperty("public_key")]
        public string PublicKey { get; set; }

        [JsonProperty("items")]
        public List<Resource> Items { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("total")]
        public int TotalItems { get; set; }

        public IEnumerator<Resource> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
