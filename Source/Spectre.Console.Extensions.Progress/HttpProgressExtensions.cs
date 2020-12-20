namespace Spectre.Console.Extensions.Progress
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public static class HttpProgressExtensions
    {
        public static HttpProgress WithHttp(
            this HttpProgress progress,
            HttpClient client,
            HttpRequestMessage requestMessage,
            string description,
            Func<Stream, Task>? callback = null)
        {
            progress.Contexts.Add(
                new HttpProgressContext()
                {
                    Client = client, RequestMessage = requestMessage, Description = description, Callback = callback,
                });
            return progress;
        }

        public static Task StartAsync(
            this HttpProgress progress,
            CancellationToken token = default)
        {
            return progress.Progress.StartAsync(async (context) =>
            {
                var tasks = progress.Contexts.Select(
                    ctx => ProgressExtensions.RunHttpReporterAsync(context, ctx, token));
                await Task.WhenAll(tasks.ToList());
            });
        }
    }
}
