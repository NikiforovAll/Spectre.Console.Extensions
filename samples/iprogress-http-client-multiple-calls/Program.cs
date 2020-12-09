namespace Samples
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Spectre.Console;
    using Spectre.Console.Extensions;

    /// <summary>
    /// Entry point.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point.
        /// </summary>
        /// <returns>Result.</returns>
        public static async Task Main() => await RunHttpClientDemoAsync();

        private static async Task RunHttpClientDemoAsync()
        {
            string[] urls =
            {
                "as5.png",
                "dotNET-bot_kamckinn_v2.png",
            };
            var client = GenerateHttpClient();
            CancellationTokenSource cts =
                new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var messagesTasks = urls
                .Select(u => client.GetAsync(
                    u, HttpCompletionOption.ResponseHeadersRead, cts.Token)).ToList();
            var work = Task.WhenAll(messagesTasks);
            var messages = messagesTasks.Select(t => t.Result.EnsureSuccessStatusCode()).ToList();
            IList<long?> totals = messages
                .Select(m => m.Content.Headers.ContentLength)
                .ToList();

            IEnumerable<ProgressTask> DeclareTasks(ProgressContext context)
            {
                yield return context.AddTask(
                    $"Content-Length: {totals[0]}", new ProgressTaskSettings() { MaxValue = totals[0] ?? -1 });
                yield return context.AddTask(
                    $"Content-Length: {totals[1]}", new ProgressTaskSettings() { MaxValue = totals[1] ?? -1 });
            }
            var files = new List<string>();
            Task DoSomeWork1(IProgress<double> p) =>
                DoSomeWorkCoreAsync(messages[0].Content, p, cts.Token)
                    .ContinueWith(ReportReceivedFile, TaskScheduler.Current);

            Task DoSomeWork2(IProgress<double> p) =>
                DoSomeWorkCoreAsync(messages[1].Content, p, cts.Token)
                    .ContinueWith(ReportReceivedFile, TaskScheduler.Current);

            void ReportReceivedFile<T>(Task<T> t)
            {
                lock (files)
                {
                    files.Add(t.Result.ToString());
                }
            }

            try
            {
                await BuildProgress().StartAsync(DeclareTasks, DoSomeWork1, DoSomeWork2);

                foreach (var fn in files)
                {
                    AnsiConsole.MarkupLine($"{Emoji.Known.CheckMarkButton} [underline grey]{fn}[/]");
                    var image = new CanvasImage(fn).MaxWidth(16);
                    AnsiConsole.Render(image);
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        private static Progress BuildProgress() =>
            AnsiConsole.Progress()
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new RemainingTimeColumn(),
                    new SpinnerColumn(),
                });

        private static async Task<string> DoSomeWorkCoreAsync(
            HttpContent content,
            IProgress<double> progress,
            CancellationToken token)
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
            using var stream = await content.ReadAsStreamAsync(token);
            using var file = new MemoryStream();
            await stream.CopyToAsync(file, 2048, progress, token);
            await File.WriteAllBytesAsync(fileName, file.ToArray(), token);
            return fileName;
        }

        private static HttpClient GenerateHttpClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://mod-dotnet-bot.net/assets/images/gallery/"),
            };
            client.DefaultRequestHeaders.Add("User-Agent", "BotScrapperFromSpectreConsole");
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
            };
            return client;
        }
    }
}
