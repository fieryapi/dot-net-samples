namespace EFI.Sample
{
    using System;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    internal class Program
    {
        private static string apiKey = @"{{the api key}}";

        private static string hostname = "{{the server name or ip address}}";

        private static string jobId = "{{job id}}";

        private static string password = "{{the password}}";

        private static string username = "{{the username}}";

        private static async Task GetJobPreviewSampleAsync(HttpClient client)
        {
            var response = await client.GetAsync("jobs/" + jobId + "/preview/1");

            Console.WriteLine();
            Console.WriteLine("Get job preview");
            var uri = string.Format("data:image/jpeg;base64,{0}",
                Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync()));
            Console.WriteLine(uri);
        }

        private static async Task GetJobsSampleAsync(HttpClient client)
        {
            var response = await client.GetAsync("jobs");

            Console.WriteLine();
            Console.WriteLine("Get jobs");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private static async Task GetSingleJobSampleAsync(HttpClient client)
        {
            var response = await client.GetAsync("jobs/" + jobId);

            Console.WriteLine();
            Console.WriteLine("Get single job");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private static async Task<HttpClient> LoginSampleAsync()
        {
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

            Console.WriteLine();
            Console.WriteLine("Login");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
            return client;
        }

        private static async Task LogoutSampleAsync(HttpClient client)
        {
            var response = await client.PostAsync("logout", null);

            Console.WriteLine();
            Console.WriteLine("Logout");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private static void Main(string[] args)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateCertificate;
            RunSampleAsync().Wait();
        }

        private static async Task PrintJobSampleAsync(HttpClient client)
        {
            var response = await client.PutAsync("jobs/" + jobId + "/print", null);

            Console.WriteLine();
            Console.WriteLine("Print a job");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private static async Task RunSampleAsync()
        {
            var client = await LoginSampleAsync();
            await GetJobsSampleAsync(client);
            await GetSingleJobSampleAsync(client);
            await PrintJobSampleAsync(client);
            await GetJobPreviewSampleAsync(client);
            await LogoutSampleAsync(client);
        }

        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
