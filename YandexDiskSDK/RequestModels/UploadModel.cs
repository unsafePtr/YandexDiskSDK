using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public class UploadModel : BaseRequestModel
    {
        public string Path { get; private set; }
        public bool? Overwrite { get; set; }
        public List<string> Fields { get; set; }

        public UploadModel(string diskPath, string fileNameWithExtension)
        {
            ThrowIfNullArgument(diskPath);
            ThrowIfNullArgument(fileNameWithExtension);

            this.Path = diskPath + fileNameWithExtension;
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
            AddParameter(this.PathParameter(this.Path));
            AddParameter("overwrite", this.Overwrite);
            AddParameter(this.FieldsParameter(this.Fields));      
        }
    }
}
