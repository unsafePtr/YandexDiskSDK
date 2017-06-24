using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK.ResponseModels
{
    public class OperationStatus
    {
        public const string Success = "success";
        public const string Failed = "failed";

        public string Status { get; set; }
    }
}
