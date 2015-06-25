dot-net-samples
===============

Fiery job management samples

**Note** Always use secure connection (HTTPS) when connecting to Fiery API in production.


### Login

```csharp
var loginJson = new JObject();
loginJson["username"] = "{{the username}}";
loginJson["password"] = "{{the password}}";
loginJson["accessrights"] = @"{{the api key}}";

var serverAddress = "https://{{the server name or ip address}}/live/api/v2/";
var client = new HttpClient { BaseAddress = new Uri(serverAddress) };

var request = new StringContent(loginJson.ToString(), Encoding.UTF8, "application/json");
var response = await client.PostAsync("login", request);
```


### Logout

```csharp
var request = new StringContent(string.Empty, Encoding.UTF8, "application/json");
var response = await client.PostAsync("logout", request);
```


### Get jobs

```csharp
var response = await client.GetAsync("jobs");
```


### Get single job

```csharp
var jobId = "{{job id}}"; // e.g. 00000000.558895DF.16055
var response = await client.GetAsync("jobs/" + jobId);
```


### Print a job

```csharp
var jobId = "{{job id}}"; // e.g. 00000000.558895DF.16055
var response = await client.PutAsync("jobs/" + jobId + "/print", null);
```


### Get job preview

```csharp
var jobId = "{{job id}}"; // e.g. 00000000.558895DF.16055
var response = await client.GetAsync("jobs/" + jobId + "/preview/1");
```
