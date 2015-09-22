.NET samples
===============

Fiery job management samples. The sample code requires .NET 4.5 Framework installed on the system.

**Note** Always use secure connection (HTTPS) when connecting to Fiery API in production.


### Login

```csharp
var loginJson = new JObject();
loginJson["username"] = username;
loginJson["password"] = password;
loginJson["accessrights"] = apiKey;

var serverAddress = string.Format(
    System.Globalization.CultureInfo.InvariantCulture,
    "https://{0}/live/api/v2/",
    hostname);
var client = new HttpClient { BaseAddress = new Uri(serverAddress) };

var request = new StringContent(loginJson.ToString(), Encoding.UTF8, "application/json");
var response = await client.PostAsync("login", request);
```


### Logout

```csharp
var request = new StringContent(string.Empty, Encoding.UTF8, "application/json");
var response = await client.PostAsync("logout", request);
```


### Create a new job

```csharp
var fullPath = @"the_job_content_full_file_path";  // e.g. d:\business_card.pdf
using (var s = new FileStream(fullPath, FileMode.Open))
{
    var request = new MultipartFormDataContent();
    request.Add(new StreamContent(s), "file", Path.GetFileName(fullPath));

    var response = await client.PostAsync("jobs", request);
}
```


### Get jobs

```csharp
var response = await client.GetAsync("jobs");
```


### Get single job

```csharp
var jobId = "the_job_id";  // e.g. 00000000.558895DF.16055
var response = await client.GetAsync("jobs/" + jobId);
```


### Print a job

```csharp
var jobId = "the_job_id";  // e.g. 00000000.558895DF.16055
var response = await client.PutAsync("jobs/" + jobId + "/print", null);
```


### Get job preview

```csharp
var jobId = "the_job_id";  // e.g. 00000000.558895DF.16055
var response = await client.GetAsync("jobs/" + jobId + "/preview/1");
```
