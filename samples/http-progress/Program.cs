namespace Samples
{
    using System;
    using System.IO;
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
        public static async Task Main()
        {
            // string url = "as5.png";
            string url = "http://speedtest-ny.turnkeyinternet.net/100mb.bin";
            var httpClient = GenerateHttpClient();
            CancellationTokenSource cts =
                new CancellationTokenSource(TimeSpan.FromMinutes(5));
            string? writtenFile = null;
            try
            {
                await BuildProgress().StartAsync(
                    httpClient,
                    new HttpRequestMessage(HttpMethod.Get, url),
                    taskDescription: url,
                    (stream) => SaveFileAsync(stream, cts.Token).ContinueWith(
                            (t) => { writtenFile = t.Result; }, TaskScheduler.Current));
                if (writtenFile is not null)
                {
                    AnsiConsole.MarkupLine(
                        $"{Emoji.Known.CheckMarkButton} [underline grey]{writtenFile}[/] {new FileInfo(writtenFile).Length}");
                    if (AnsiConsole.Confirm("Want to delete file?"))
                    {
                        File.Delete(writtenFile);
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        private static async Task<string> SaveFileAsync(
            Stream inputStream,
            CancellationToken token)
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".bin";
            var memstream = new MemoryStream();
            await inputStream.CopyToAsync(memstream, token);
            await inputStream.FlushAsync(token);
            await File.WriteAllBytesAsync(fileName, memstream.ToArray(), token);
            return fileName;
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

        private static HttpClient GenerateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "BotScrapperFromSpectreConsole");
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
            {
                NoCache = true,
            };
            return client;
        }
    }
}
