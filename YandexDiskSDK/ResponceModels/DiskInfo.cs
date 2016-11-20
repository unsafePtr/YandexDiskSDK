using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK.ResponceModels
{
    public class DiskInfo
    {
        [JsonProperty("trash_size")]
        public long TrashSize { get; set; }

        [JsonProperty("total_space")]
        public long TotalSpace { get; set; }

        [JsonProperty("used_space")]
        public long UsedSpace { get; set; }

        [JsonProperty("system_folders")]
        public Dictionary<string, string> Folders { get; set; }
        
    }
}
