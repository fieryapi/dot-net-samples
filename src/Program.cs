namespace EFI.Sample
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    internal class Program
    {
        /// <summary>
        /// Set the key to access Fiery API
        /// </summary>
        private static string apiKey = @"the_api_key";

        /// <summary>
        /// Set the full file path for job submission
        /// </summary>
        private static string fullPath = @"the_job_content_full_file_path";

        /// <summary>
        /// Set the host name as fiery server name or ip address
        /// </summary>
        private static string hostname = "the_server_name_or_ip_address";

        /// <summary>
        /// Set the job id on the fiery server to retrieve job information and preview
        /// </summary>
        private static string jobId = "the_job_id";

        /// <summary>
        /// Set the password to login to the fiery server
        /// </summary>
        private static string password = "the_password";

        /// <summary>
        /// Set the username to login to the fiery server
        /// </summary>
        private static string username = "the_username";

        /// <summary>
        /// Set the first page preview of the job
        /// </summary>
        /// <param name="client">The HTTP client with valid session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private static async Task GetJobPreviewSampleAsync(HttpClient client)
        {
            var response = await client.GetAsync("jobs/" + jobId + "/preview/1");

            Console.WriteLine();
            Console.WriteLine("Get job preview");
            var uri = string.Format("data:image/jpeg;base64,{0}",
                Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync()));
            Console.WriteLine(uri);
        }

        /// <summary>
        /// Get job information from all jobs on the fiery server
        /// </summary>
        /// <param name="client">The HTTP client with valid session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private static async Task GetJobsSampleAsync(HttpClient client)
        {
            var response = await client.GetAsync("jobs");

            Console.WriteLine();
            Console.WriteLine("Get jobs");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Get job information of a single job on the fiery server
        /// </summary>
        /// <param name="client">The HTTP client with valid session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private static async Task GetSingleJobSampleAsync(HttpClient client)
        {
            var response = await client.GetAsync("jobs/" + jobId);

            Console.WriteLine();
            Console.WriteLine("Get single job");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Login to the fiery server
        /// </summary>
        /// <param name="client">The HTTP client with valid session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Logout from the fiery server
        /// </summary>
        /// <param name="client">The HTTP client with valid session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private static async Task LogoutSampleAsync(HttpClient client)
        {
            var response = await client.PostAsync("logout", null);

            Console.WriteLine();
            Console.WriteLine("Logout");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Entry point of the application
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        private static void Main(string[] args)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = ValidateCertificate;
            RunSampleAsync().Wait();
        }

        /// <summary>
        /// Create a new job on the fiery server
        /// </summary>
        /// <param name="client">The HTTP client with valid session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private static async Task PostJobContentSampleAsync(HttpClient client)
        {
            using (var s = new FileStream(fullPath, FileMode.Open))
            {
                var request = new MultipartFormDataContent();
                request.Add(new StreamContent(s), "file", Path.GetFileName(fullPath));

                // override default number of copies to 10 copies
                request.Add(new StringContent("10"), "\"attributes[num copies]\"");

                var response = await client.PostAsync("jobs", request);

                Console.WriteLine();
                Console.WriteLine("Submit a new job");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        /// <summary>
        /// Send a print action to a job on the fiery server
        /// </summary>
        /// <param name="client">The HTTP client with valid session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private static async Task PrintJobSampleAsync(HttpClient client)
        {
            var response = await client.PutAsync("jobs/" + jobId + "/print", null);

            Console.WriteLine();
            Console.WriteLine("Print a job");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Main method executing all sample code
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private static async Task RunSampleAsync()
        {

            var client = await LoginSampleAsync();
            await PostJobContentSampleAsync(client);
            await GetJobsSampleAsync(client);
            await GetSingleJobSampleAsync(client);
            await UpdateJobAttributeAsync(client);
            await PrintJobSampleAsync(client);
            await GetJobPreviewSampleAsync(client);
            await LogoutSampleAsync(client);
        }

        /// <summary>
        /// Update a job attribute value on the fiery server.
        /// </summary>
        /// <param name="client">The HTTP client with valid session.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        private static async Task UpdateJobAttributeAsync(HttpClient client)
        {
            ////// note - multipart/form-data is supported but not recommended.
            ////var request = new MultipartFormDataContent();
            ////request.Add(new StringContent("1"), "\"attributes[num copies]\"");

            var attributeJson = new JObject();
            attributeJson["num copies"] = "1";

            var jobJson = new JObject();
            jobJson["attributes"] = attributeJson;

            var request = new StringContent(jobJson.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("jobs/" + jobId, request);

            Console.WriteLine();
            Console.WriteLine("Update a job");
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Ignore all certificates errors when sending request to the fiery server. Using this method without validation on production environment will increase the risk of MITM attack.
        /// </summary>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="certificate">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>A value that determines whether the specified certificate is accepted for authentication.</returns>
        private static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
