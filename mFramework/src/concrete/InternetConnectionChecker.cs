using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace mFramework.concrete
{
    public class InternetConnectionChecker
    {
        public static void PingAsync(Action onSuccess, Action<Exception> onFailed, int timeout = 10000)
        {
            try
            {
                var ping = new Ping();
                var host = "8.8.8.8";
                var buffer = new byte[32];
                var pingOptions = new PingOptions();
                var result = ping.SendPingAsync(host, timeout, buffer, pingOptions);

                result.ContinueWith(reply =>
                {
                    if (result.Result.Status == IPStatus.Success)
                        onSuccess?.Invoke();
                    else
                        onFailed?.Invoke(result.Exception);
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                result.ContinueWith(reply =>
                {
                    onFailed?.Invoke(reply.Exception);
                }, TaskContinuationOptions.OnlyOnFaulted);

                result.ContinueWith(reply =>
                {
                    onFailed?.Invoke(reply.Exception);
                }, TaskContinuationOptions.OnlyOnCanceled);
            }
            catch (Exception exception)
            {
                onFailed?.Invoke(exception);
            }
        }

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

            result.ContinueWith(task =>
            {
                onFailed?.Invoke(task.Exception);
            }, TaskContinuationOptions.OnlyOnCanceled);
        }

        private static async Task<bool> GetPageAsync(Uri uri)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                
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