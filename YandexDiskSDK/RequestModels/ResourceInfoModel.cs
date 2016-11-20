using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public enum SortBy { Name, Path, Created, Modified, Size }
    public enum PreviewSize { S = 150, M = 300, L = 500, XL = 800, XXL = 1024, XXXL = 1280 }
    public enum SortDirection { Ascending, Descending }

    public class ResourceInfoModel : BaseRequestModel
    {
        public string Path { get; private set; }
        public SortBy? SortBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
        public List<string> Fields { get; set; }
        public PreviewSize? PreviewSize { get; set; }
        public bool? PreviewCrop { get; set; }

        public ResourceInfoModel(string path)
        {
            ThrowIfNullArgument(path);

            this.Path = path;
            this.SortDirection = SortDirection.Ascending;
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
            AddParameter(this.SortParameter(this.SortBy, this.SortDirection));
            AddParameter("limit", this.Limit);
            AddParameter("offset", this.Offset);
            AddParameter(this.FieldsParameter(this.Fields));
            AddParameter("preview_size", this.PreviewSize);
            AddParameter("preview_crop", this.PreviewCrop);
        }
    }
}
