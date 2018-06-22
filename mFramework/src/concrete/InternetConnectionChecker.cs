using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace mFramework.concrete
{
    public class InternetConnectionChecker
    {
        public static async void Check(Action onSuccess, Action onFailed)
        {
            var result = await GetPage(new Uri("http://google.com"));
            if (result)
                onSuccess?.Invoke();
            else 
                onFailed?.Invoke();
        }

        private static async Task<bool> GetPage(Uri uri)
        {
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(uri))
                {
                    var success = (int) response.StatusCode >= 200 && (int) response.StatusCode < 299;
                    if (!success)
                        return false;

                    using (var stream = new StreamReader(await response.Content.ReadAsStreamAsync()))
                    {
                        try
                        {
                            var buffer = new char[80];
                            stream.Read(buffer, 0, buffer.Length);
                            return new string(buffer).Contains("schema.org/WebPage");
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }
        }
    }
}