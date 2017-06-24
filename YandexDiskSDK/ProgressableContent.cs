using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YandexDiskSDK
{
    public class ProgressableContent : HttpContent
    {
        private const int chunkSize = 4096;
        private byte[] content;
        private Action<decimal> progress;

        public ProgressableContent(byte[] content, Action<decimal> progress)
        {
            ThrowIfNullArgument(content);
            ThrowIfNullArgument(progress);

            this.content = content;
            this.progress = progress;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            for (int i = 0; i < this.content.Length; i += chunkSize)
            {
                await stream.WriteAsync(this.content, i, Math.Min(chunkSize, this.content.Length - i)).ConfigureAwait(false);
                this.progress(100m * (i + 4096) / this.content.Length);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = this.content.Length;
            return true;
        }

        public void ThrowIfNullArgument(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
        }
    }
}
