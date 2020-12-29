namespace Spectre.Console.Extensions.Progress
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Spectre Progress extensions methods to build further on <see cref="HttpProgress"/>.
    /// </summary>
    public static class HttpProgressExtensions
    {
        /// <summary>
        /// Populates underlying <see cref="HttpProgress"/> container by mutating context collection.
        /// </summary>
        /// <param name="progress">A <see cref="HttpProgress"/>.</param>
        /// <param name="client">A <see cref="HttpClient"/>.</param>
        /// <param name="requestMessage">A <see cref="HttpRequestMessage"/>.</param>
        /// <param name="description">A label for an operation.</param>
        /// <param name="callback">An operation to process operation result.</param>
        /// <returns>A mutated <see cref="HttpProgress"/>.</returns>
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

        /// <summary>
        /// Starts configured <see cref="HttpProgress"/>.
        /// </summary>
        /// <param name="progress">Underlying <see cref="HttpProgress"/>.</param>
        /// <param name="token">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
