using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace mFramework.concrete
{
    public class InternetConnectionChecker
    {
        public static void CheckAsync(Action onSuccess, Action<Exception> onFailed)
        {
            var result = GetPageAsync(new Uri("http://google.com"));

            result.ContinueWith(task =>
            {
                if (task.Result)
                    onSuccess?.Invoke();
                else
                    onFailed?.Invoke(task.Exception);
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            result.ContinueWith(task =>
            {
                onFailed?.Invoke(task.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private static async Task<bool> GetPageAsync(Uri uri)
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