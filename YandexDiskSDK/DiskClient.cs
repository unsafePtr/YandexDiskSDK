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
            this.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("OAuth", token);
            
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
            using (var response = await client.GetAsync(api))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.OK);

                return await ReadAsAsync<DiskInfo>(response);
            }
        }

        public async Task<Resource> ResourceInformationAsync(ResourceInfoModel model)
        {
            return await GetAsync<ResourceInfoModel, Resource>(model);
        }

        public async Task<FilesResourceList> SearchAsync(SearchModel model)
        {
            return await GetAsync<SearchModel, FilesResourceList>(model);
        }

        public async Task<LastUploadedResourceList> LastUploadedAsync(LastUploadedModel model)
        {
            return await GetAsync<LastUploadedModel, LastUploadedResourceList>(model);
        }

        public async Task UploadAsync(UploadModel model, byte[] file)
        {
            ThrowIfNullArgument(file);

            Link link = await GetAsync<UploadModel, Link>(model);
            ByteArrayContent content = new ByteArrayContent(file);
            using (var response = await client.PutAsync(link.Href, content))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.Created);
            }
        }

        public async Task UploadAsync(UploadModel model, byte[] file, Action<decimal> progressCallback)
        {
            ThrowIfNullArgument(file);
            ThrowIfNullArgument(progressCallback);

            Link link = await GetAsync<UploadModel, Link>(model);
            ProgressableContent content = new ProgressableContent(file, progressCallback);
            using (var response = await client.PutAsync(link.Href, content))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.Created);
            }
        }        

        public async Task<Link> UploadFromSourceAsync(UploadFromSourceModel model)
        {
            ThrowIfNullArgument(model);

            string url = model.RequestUrl(api);
            using (var response = await client.PostAsync(url, null))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.Accepted);
                
                return await ReadAsAsync<Link>(response);
            }
        }

        public async Task<byte[]> DownloadAsync(DownloadModel model)
        {
            Link link = await GetAsync<DownloadModel, Link>(model);
            using (var response = await client.GetAsync(link.Href))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.OK);

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public async Task<byte[]> DownloadAsync(DownloadModel model, Action<decimal> progressCallback)
        {
            ThrowIfNullArgument(progressCallback);

            Link link = await GetAsync<DownloadModel, Link>(model);
            using (var response = await client.GetAsync(link.Href, HttpCompletionOption.ResponseHeadersRead))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.OK);
                
                long? totalBytes = response.Content.Headers.ContentLength.Value;
                // rest api always return non-nullable ContentLength
                // does not need to check for null
                //if (totalBytes == null)
                //    throw new Exception("can't download with progress");

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    const int bufferSize = 4096;
                    byte[] buffer = new byte[bufferSize];
                    long totalReaded = 0L;
                    int bufferBytesReaded = 0;
                    
                    while ((bufferBytesReaded = await stream.ReadAsync(buffer, 0, bufferSize)) > 0)
                    {
                        await memoryStream.WriteAsync(buffer, 0, bufferBytesReaded);
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
            using (var response = await client.DeleteAsync(url))
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return null;
                }
                else if(response.StatusCode == HttpStatusCode.Accepted)
                {
                    return await ReadAsAsync<Link>(response);
                }
                else
                {
                    var error = await ReadAsAsync<ErrorResponse>(response);
                    throw new HttpDiskException(error);
                }
            }
        }

        public async Task<Link> CreateFolderAsync(CreateFolderModel model)
        {
            ThrowIfNullArgument(model);

            string url = model.RequestUrl(api);
            using (var response = await client.PutAsync(url, null))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.Created);
                
                return await ReadAsAsync<Link>(response);
            }
        }
        
        public async Task<OperationStatus> OperationStatusAsync(Link link)
        {
            ThrowIfNullArgument(link);

            using (var response = await client.GetAsync(link.Href))
            {
                await ThrowIfIsNotExpectedResponseCode(response, HttpStatusCode.OK);
                return await ReadAsAsync<OperationStatus>(response);
            }
        }

        #endregion implementation
        #region generics
        private async Task<T> ReadAsAsync<T>(HttpResponseMessage httpResponse)
        {
            return await httpResponse.Content.ReadAsAsync<T>(formatters);
        }

        private async Task<TResponseModel> GetAsync<TRequestModel, TResponseModel>(TRequestModel model)
            where TRequestModel : BaseRequestModel
            where TResponseModel : class
        {
            return await GetAsync<TRequestModel, TResponseModel>(model, HttpStatusCode.OK);
        }

        private async Task<TResponseModel> GetAsync<TRequestModel, TResponseModel>(TRequestModel model, HttpStatusCode expectedCode)
            where TRequestModel : BaseRequestModel
            where TResponseModel : class
        {
            ThrowIfNullArgument(model);

            string url = model.RequestUrl(api);
            using (var response = await client.GetAsync(url))
            {
                await ThrowIfIsNotExpectedResponseCode(response, expectedCode);

                return await ReadAsAsync<TResponseModel>(response);
            }
        }

        #endregion generics
        #region exceptions
        private void ThrowIfNullArgument(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
        }
        
        private async Task ThrowIfIsNotExpectedResponseCode(HttpResponseMessage response, HttpStatusCode code)
        {
            if(response.StatusCode != code)
            {
                var error = await ReadAsAsync<ErrorResponse>(response);
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
