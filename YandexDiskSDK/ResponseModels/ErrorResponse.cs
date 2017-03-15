using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK.ResponseModels
{
    public class ErrorResponse
    {
        public string Error { get; set; }
        public string Description { get; set; }
    }
}
