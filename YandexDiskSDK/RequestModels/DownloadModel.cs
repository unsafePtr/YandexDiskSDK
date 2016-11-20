using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public class DownloadModel : BaseRequestModel
    {
        public string Path { get; set; }
        public List<string> Fields { get; set; }

        public DownloadModel(string path)
        {
            ThrowIfNullArgument(path);

            this.Path = path;
        }        

        protected override string PathSuffix
        {
            get
            {
                return "resources/download";
            }
        }

        protected override void AddRequestParameters()
        {
            AddParameter(this.PathParameter(this.Path));
            AddParameter(this.FieldsParameter(this.Fields));
        }
    }
}
