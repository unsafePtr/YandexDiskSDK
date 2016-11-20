using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public class CreateFolderModel : BaseRequestModel
    {
        public string Path { get; private set; }
        public List<string> Fields { get; set; }

        public CreateFolderModel(string path, string folderName)
        {
            ThrowIfNullArgument(path);
            ThrowIfNullArgument(folderName);

            this.Path = path + folderName;
        }

        protected override string PathSuffix
        {
            get
            {
                return "resources";
            }
        }

        protected override void AddRequestParameters()
        {
            AddParameter(this.PathParameter(this.Path));
            AddParameter(this.FieldsParameter(this.Fields));
        }
    }
}
