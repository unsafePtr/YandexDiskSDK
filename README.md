# Description
A library for providing simple working operation with Yandex Disk written in c# with asynchronous manner. Library communicates with [cloud REST API](https://tech.yandex.com/disk/rest/).
# Quickstart
DiskClient is the main class for communication with api, which contains several constructors. The main requries [OAuth token](https://tech.yandex.com/oauth/). If you don't have it you can recive it with helper class YandexOAuth.
```c#
DiskClient client = new DiskClient("your token here");

// YandexOAuth
YandexOAuth yandexAuth = new YandexAuth();
string token = await yandexAuth.GetTokenAsync(confirmationCode, clientId, clientSecret);
DiskClient client = new DiskClient(token);
```

And there is constructors where you can inject already created HttpClient or proper JsonMediaTypeFormatter.
```c#
DiskClient client = new DiskClient("token", new HttpClient());
```

Almost all methods to communicate with api require request model.
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
