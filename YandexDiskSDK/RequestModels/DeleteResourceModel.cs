using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public class DeleteResourceModel : BaseRequestModel
    {
        public string Path { get; private set; }
        public bool? Permanently { get; set; }
        public List<string> Fields { get; set; }
        
        public DeleteResourceModel(string path)
        {
            ThrowIfNullArgument(path);
            this.Path = path;
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
            AddParameter("permanently", this.Permanently);
            AddParameter(this.FieldsParameter(this.Fields));
        }
    }
}
