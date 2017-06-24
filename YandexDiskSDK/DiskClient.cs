using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YandexDiskSDK.Exceptions;
using YandexDiskSDK.ResponseModels;
using YandexDiskSDK.RequestModels;
using System.Net.Http.Formatting;
using System.IO;
using System.Threading;
using System.Net.Http.Headers;

namespace YandexDiskSDK
{
    public class DiskClient : IDisposable
    {
        #region init

        private const string api = "https://cloud-api.yandex.net/v1/disk/";
        private HttpClient client;
        private List<MediaTypeFormatter> formatters;
        
        public DiskClient(string token) : this(token, null) { }

        public DiskClient(string token, JsonMediaTypeFormatter jsonFormatter)
        {
            ThrowIfNullArgument(token);

            client = new HttpClient();
            this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
            
            /* HttpResponseMessage.Content.ReadAsAsync<T> use by default IEnumerable<MediaTypeFormatter>
             * This enumerable contains JsonMediaTypeFormatter, XmlMediaTypeFormatter, FormUrlEncodedMediaTypeFormatter
             * All responses from Yandex Disk REST Api are with json mediatype header
             * Then we does not need others MediaType formatters
             * JsonMediaTypeFormatter use by default Newtonsoft json serializer.
             * If you are want to use your own serializer, implement class which inherit JsonMediaTypeFormatter
             */
            formatters = new List<MediaTypeFormatter>(1);
            formatters.Add(jsonFormatter ?? new JsonMediaTypeFormatter());
        }

        #endregion init
        #region implementation

        public async Task<DiskInfo> DiskInformationAsync()
        {
            using (var response = await client.GetAsync(api).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.OK).ConfigureAwait(false);

                return await ReadAsAsync<DiskInfo>(response).ConfigureAwait(false);
            }
        }

        public Task<Resource> ResourceInformationAsync(ResourceInfoModel model)
        {
            return GetAsync<ResourceInfoModel, Resource>(model);
        }

        public Task<FilesResourceList> SearchAsync(SearchModel model)
        {
            return GetAsync<SearchModel, FilesResourceList>(model);
        }

        public Task<LastUploadedResourceList> LastUploadedAsync(LastUploadedModel model)
        {
            return GetAsync<LastUploadedModel, LastUploadedResourceList>(model);
        }

        public async Task UploadAsync(UploadModel model, byte[] file)
        {
            await UploadAsync(model, file, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task UploadAsync(UploadModel model, byte[] file, CancellationToken cancellationToken)
        {
            ThrowIfNullArgument(file);
            
            Link link = await GetAsync<UploadModel, Link>(model).ConfigureAwait(false);
            ByteArrayContent content = new ByteArrayContent(file);
            using (var response = await client.PutAsync(link.Href, content, cancellationToken).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.Created).ConfigureAwait(false);
            }
        }

        public async Task UploadAsync(UploadModel model, byte[] file, Action<decimal> progressCallback)
        {
            await UploadAsync(model, file, progressCallback, CancellationToken.None).ConfigureAwait(false);
        }

        public async Task UploadAsync(UploadModel model, byte[] file, Action<decimal> progressCallback, CancellationToken cancellationToken)
        {
            ThrowIfNullArgument(file);
            ThrowIfNullArgument(progressCallback);

            Link link = await GetAsync<UploadModel, Link>(model).ConfigureAwait(false);
            ProgressableContent content = new ProgressableContent(file, progressCallback);
            using (var response = await client.PutAsync(link.Href, content, cancellationToken).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.Created).ConfigureAwait(false);
            }
        }

        public async Task<bool> UploadFromSourceAsync(UploadFromSourceModel model, TimeSpan? checkTiming = null)
        {
            ThrowIfNullArgument(model);

            checkTiming = checkTiming ?? new TimeSpan(0, 0, 10);

            string url = model.RequestUrl(api);
            using (var response = await client.PostAsync(url, null).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.Accepted).ConfigureAwait(false);
                Link pendingResourceLink = await ReadAsAsync<Link>(response).ConfigureAwait(false);
                
                while (true)
                {
                    OperationStatus operationStatus = await OperationStatusAsync(pendingResourceLink).ConfigureAwait(false);

                    switch (operationStatus.Status)
                    {
                        case OperationStatus.Success:
                            return true;
                        case OperationStatus.Failed:
                            return false;
                        default:
                            await Task.Delay((TimeSpan)checkTiming);
                            break;
                    }
                }
            }
        }

        public Task<byte[]> DownloadAsync(DownloadModel model)
        {
            return DownloadAsync(model, CancellationToken.None);
        }

        public async Task<byte[]> DownloadAsync(DownloadModel model, CancellationToken cancellationToken)
        {
            Link link = await GetAsync<DownloadModel, Link>(model).ConfigureAwait(false);
            using (var response = await client.GetAsync(link.Href, cancellationToken).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.OK).ConfigureAwait(false);

                return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
        }

        public Task<byte[]> DownloadAsync(DownloadModel model, Action<decimal> progressCallback)
        {
            return DownloadAsync(model, progressCallback, CancellationToken.None);
        }

        public async Task<byte[]> DownloadAsync(DownloadModel model, Action<decimal> progressCallback, CancellationToken cancellationToken)
        {
            ThrowIfNullArgument(progressCallback);

            Link link = await GetAsync<DownloadModel, Link>(model).ConfigureAwait(false);
            using (var response = await client.GetAsync(link.Href, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.OK).ConfigureAwait(false);

                long? totalBytes = response.Content.Headers.ContentLength.Value;
                // rest api always return non-nullable ContentLength
                // does not need to check for null
                //if (totalBytes == null)
                //    throw new Exception("can't download with progress");

                using (Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    const int bufferSize = 4096;
                    byte[] buffer = new byte[bufferSize];
                    long totalReaded = 0L;
                    int bufferBytesReaded = 0;

                    while ((bufferBytesReaded = await stream.ReadAsync(buffer, 0, bufferSize).ConfigureAwait(false)) > 0)
                    {
                        await memoryStream.WriteAsync(buffer, 0, bufferBytesReaded).ConfigureAwait(false);
                        totalReaded += bufferBytesReaded;

                        decimal report = 100m * totalReaded / (long)totalBytes;
                        progressCallback(report);
                    }

                    return memoryStream.ToArray();
                }
            }
        }
        
        public async Task<Link> DeleteAsync(DeleteResourceModel model)
        {
            ThrowIfNullArgument(model);

            string url = model.RequestUrl(api);
            using (var response = await client.DeleteAsync(url).ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return null;
                }
                else if(response.StatusCode == HttpStatusCode.Accepted)
                {
                    return await ReadAsAsync<Link>(response).ConfigureAwait(false);
                }
                else
                {
                    var error = await ReadAsAsync<ErrorResponse>(response).ConfigureAwait(false);
                    throw new HttpDiskException(error);
                }
            }
        }

        public async Task<Link> CreateFolderAsync(CreateFolderModel model)
        {
            ThrowIfNullArgument(model);

            string url = model.RequestUrl(api);
            using (var response = await client.PutAsync(url, null).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.Created).ConfigureAwait(false);
                
                return await ReadAsAsync<Link>(response).ConfigureAwait(false);
            }
        }
        
        public async Task<OperationStatus> OperationStatusAsync(Link link)
        {
            ThrowIfNullArgument(link);

            using (var response = await client.GetAsync(link.Href).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.OK).ConfigureAwait(false);
                return await ReadAsAsync<OperationStatus>(response).ConfigureAwait(false);
            }
        }

        #endregion implementation
        #region generics
        private Task<T> ReadAsAsync<T>(HttpResponseMessage httpResponse)
        {
            return httpResponse.Content.ReadAsAsync<T>(formatters);
        }

        private Task<TResponseModel> GetAsync<TRequestModel, TResponseModel>(TRequestModel model)
            where TRequestModel : BaseRequestModel
            where TResponseModel : class
        {
            return GetAsync<TRequestModel, TResponseModel>(model, HttpStatusCode.OK);
        }

        private async Task<TResponseModel> GetAsync<TRequestModel, TResponseModel>(TRequestModel model, HttpStatusCode expectedCode)
            where TRequestModel : BaseRequestModel
            where TResponseModel : class
        {
            ThrowIfNullArgument(model);

            string url = model.RequestUrl(api);
            using (var response = await client.GetAsync(url).ConfigureAwait(false))
            {
                await ThrowIfIsNotExpectedResponseCode(response, expectedCode).ConfigureAwait(false);

                return await ReadAsAsync<TResponseModel>(response).ConfigureAwait(false);
            }
        }

        #endregion generics
        #region exceptions
        private void ThrowIfNullArgument<T>(T obj)
            where T : class
        {
            if(obj == null)
            {
                throw new ArgumentNullException();
            }
        }

        private void ThrowIfNullArgument<T>(Nullable<T> obj)
            where T : struct
        {
            if (!obj.HasValue)
            {
                throw new ArgumentNullException();
            }
        }
        
        private async Task ThrowIfIsNotExpectedResponseCode(HttpResponseMessage response, HttpStatusCode code)
        {
            if(response.StatusCode != code)
            {
                var error = await ReadAsAsync<ErrorResponse>(response).ConfigureAwait(false);
                throw new HttpDiskException(error);
            }
        }
        #endregion exceptions

        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}
