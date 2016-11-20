using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public class SearchModel : BaseRequestModel
    {
        public int? Limit { get; set; } // default 20 by api
        public List<MediaType> MediaTypes { get; set; }
        public int? Offset { get; set; }
        public List<string> Fields { get; set; }
        public PreviewSize? PreviewSize { get; set; }
        public bool? PreviewCrop { get; set; }

        public SearchModel() { }

        protected override string PathSuffix
        {
            get
            {
                return "resources/files";
            }
        }

        protected override void AddRequestParameters()
        {
            AddParameter("limit", this.Limit);
            AddParameter(this.MediaTypeParameter(this.MediaTypes));
            AddParameter("offset", this.Offset);
            AddParameter(this.FieldsParameter(this.Fields));
            AddParameter("preview_size", this.PreviewSize);
            AddParameter("preview_crop", this.PreviewCrop);
        }        
    }
}
