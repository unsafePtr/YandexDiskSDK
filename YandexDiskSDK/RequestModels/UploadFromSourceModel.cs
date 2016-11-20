using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public class UploadFromSourceModel : BaseRequestModel
    {
        public string Url { get; private set; }
        public string Path { get; private set; }
        public List<string> Fileds { get; set; }
        public bool? DisableRedirects { get; set; }

        public UploadFromSourceModel(string fileUrl, string diskPath)
        {
            ThrowIfNullArgument(fileUrl);
            ThrowIfNullArgument(diskPath);

            this.Url = fileUrl;
            this.Path = diskPath;
        }

        protected override string PathSuffix
        {
            get
            {
                return "resources/upload";
            }
        }

        protected override void AddRequestParameters()
        {
            AddParameter("url", () => WebUtility.UrlEncode(this.Url));
            AddParameter(this.PathParameter(this.Path));
            AddParameter(this.FieldsParameter(this.Fileds));
            AddParameter("disable_redirects", this.DisableRedirects);
        }
    }
}
