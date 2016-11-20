using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Extensions.RequestParameters;

namespace YandexDiskSDK.RequestModels
{
    public enum CopyMove { Copy, Move }
    public class CopyOrMoveFolderModel : BaseRequestModel
    {
        public string FromPath { get; private set; }
        public string ToPath { get; private set; }
        public string Overwrite { get; set; }
        public List<string> Fields { get; set; }
        public CopyMove Operation { get; set; }

        public CopyOrMoveFolderModel(string fromPath, string toPath, CopyMove operation)
        {
            ThrowIfNullArgument(fromPath);
            ThrowIfNullArgument(toPath);

            this.FromPath = fromPath;
            this.ToPath = toPath;
            this.Operation = operation;
        }

        protected override string PathSuffix
        {
            get
            {
                if(Operation == CopyMove.Copy)
                {
                    return "resources/copy";
                }
                else
                {
                    return "resources/move";
                }
            }
        }

        protected override void AddRequestParameters()
        {
            AddParameter("from", () => WebUtility.UrlEncode(this.FromPath));
            AddParameter(this.PathParameter(this.ToPath));
            AddParameter("overwrite", this.Overwrite);
            AddParameter(this.FieldsParameter(this.Fields));
        }
    }
}
