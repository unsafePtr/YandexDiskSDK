# Repository is archived 
Since this repository wasn't updated for a long time is achieved in favor of [another project for Yandex Disk](https://github.com/raidenyn/yandexdisk.client). It provides the same functionality with some additional features and is actively maintained.

# Description
A library for providing simple working operation with Yandex Disk written in c# with asynchronous manner. Library communicates with [cloud REST API](https://tech.yandex.com/disk/rest/).
# Quickstart
DiskClient is the main class for communication with api, which contains several constructors. The main requires [OAuth token](https://tech.yandex.com/oauth/). If you don't have it you can receive it with helper class YandexOAuth.
```c#
DiskClient client = new DiskClient("your token here");

// YandexOAuth
YandexOAuth yandexAuth = new YandexAuth();
string token = await yandexAuth.GetTokenAsync(confirmationCode, clientId, clientSecret);
DiskClient client = new DiskClient(token);
```
Instead of creating new HttpClient you can use one provided by your app for better performance. [In fact, HttpClient is designed specifically to persist and be used for multiple requests.](https://codereview.stackexchange.com/a/69954) Otherwise, HttpClient will be created by DiskClient internally.
```c#
DiskClient client = new DiskClient(token, new HttpClient())
```

And there is constructor where you can inject your own JsonMediaTypeFormatter.
```c#
DiskClient client = new DiskClient("token", new CustomeJsonMediaTypeFormatter());
```

Almost all methods to communicate with API require request model.
```c#
UploadModel model = new UploadModel("/", "YandexTest.zip");
model.Overwrite = true;
await client.UploadAsync(model, File.ReadAllBytes(@"D:/YandexTest.zip"), (progress) =>
{
    Console.Write($"\r{Math.Round(progress, 2)}");
});
```

And response almost return response model:
```c#
ResourceInfoModel model = new ResourceInfoModel("/folder");
Resource resource = await client.ResourceInformationAsync(model);
Console.WriteLine(resource.Created);
```
Models are divided in namespaces YandexDiskSDK.RequestModels and YandexDiskSDK.ResponceModels

# Not implemented feautures
* [Adding metainformation for a resource](https://tech.yandex.com/disk/api/reference/meta-add-docpage/)
* [Working with publishing resource](https://tech.yandex.com/disk/api/reference/publish-docpage/)
* [Working with trash](https://tech.yandex.com/disk/api/reference/trash-delete-docpage/)
