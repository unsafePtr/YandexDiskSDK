using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public class LastUploadedModel : BaseRequestModel
    {
        public int? Limit { get; set; }
        public List<MediaType> MediaTypes { get; set; }
        public List<string> Fields { get; set; }
        public PreviewSize? PreviewSize { get; set; }
        public bool? PreviewCrop { get; set; }

        protected override string PathSuffix
        {
            get
            {
                return "resources/last-uploaded";
            }
        }

        protected override void AddRequestParameters()
        {
            AddParameter("limit", this.Limit);
            AddParameter(this.MediaTypeParameter(this.MediaTypes));
            AddParameter(this.FieldsParameter(this.Fields));
            AddParameter("preview_size", this.PreviewSize);
            AddParameter("preview_crop", this.PreviewCrop);
        }
    }
}
