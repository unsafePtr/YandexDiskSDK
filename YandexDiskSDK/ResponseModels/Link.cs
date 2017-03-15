using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK.ResponseModels
{
    public class Link
    {
        public string Href { get; set; }
        public string Method { get; set; }
        public bool Templated { get; set; }
    }
}
