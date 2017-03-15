using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.ResponseModels;

namespace YandexDiskSDK.Exceptions
{
    public class HttpDiskException : Exception
    {
        public HttpDiskException(ErrorResponse model): base($"{model.Error}: {model.Description}") { }
    }
}
