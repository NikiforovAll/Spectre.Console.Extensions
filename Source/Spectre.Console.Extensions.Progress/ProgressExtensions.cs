namespace Spectre.Console.Extensions.Progress
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Spectre.Console.Extensions.Progress.Helpers;
    using Progress = Spectre.Console.Progress;

    /// <summary>
    /// Spectre Progress extensions methods.
    /// </summary>
    public static class ProgressExtensions
    {
        /// <summary>
        /// Starts a <see cref="Progress"/> based on provided payload.
        /// </summary>
        /// <param name="progress">Underlying display element.</param>
        /// <param name="taskFactory">A function to build <see cref="ProgressTask"/> sequence.</param>
        /// <param name="actions">A sequence of reportable callback operations. </param>
        /// <remarks>
        /// <paramref name="taskFactory"/> produces a sequence of
        /// progress tasks which will be juxtaposed to provided callbacks.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when it is not possible to report for a given input params.</exception>
        /// <returns> A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task StartAsync(
            this Progress progress,
            Func<ProgressContext, IEnumerable<ProgressTask>> taskFactory,
            params Func<IProgress<double>, Task>[] actions)
        {
            return progress.StartAsync(async (context) =>
            {
                var rendarableTasks = taskFactory
                    .Invoke(context)?.ToArray();
                _ = rendarableTasks ?? throw new ArgumentException(
                    "Factory should return valid task collection.", nameof(taskFactory));
                if (rendarableTasks.Length != actions.Length)
                {
                    throw new ArgumentException(
                        "Number of tasks produced by factory should match the number of provided actions",
                        nameof(actions));
                }

                var tasks = new List<Task>(rendarableTasks.Length);
                for (var i = 0; i < rendarableTasks.Length; i++)
                {
                    ProgressTask rendarableTask = rendarableTasks[i];
                    var arg = new Progress<double>((delta) =>
                    {
                        rendarableTask.Increment(delta);
                    });
                    var taskResult = actions[i].Invoke(arg);
                    tasks.Add(taskResult);
                }

                await Task.WhenAll(tasks);
            });
        }

        /// <summary>
        /// Starts a <see cref="Progress"/> based on provided payload.
        /// </summary>
        /// <param name="progress">Underlying display element.</param>
        /// <param name="taskFactory">A function to build a single <see cref="ProgressTask"/>.</param>
        /// <param name="action">A reportable callback.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static Task StartAsync(
            this Progress progress,
            Func<ProgressContext, ProgressTask> taskFactory,
            Func<IProgress<double>, Task> action)
        {
            ProgressTask[] NewFactory(ProgressContext ctx) =>
                new[] { taskFactory.Invoke(ctx) ?? throw new ArgumentException(nameof(StartAsync), nameof(taskFactory)) };
            return progress.StartAsync(NewFactory, action);
        }

        /// <summary>
        /// Starts a <see cref="Progress"/> for a single <see cref="HttpClient"/> operation.
        /// </summary>
        /// <param name="progress">Underlying display element.</param>
        /// <param name="client">A client.</param>
        /// <param name="request">A request.</param>
        /// <param name="taskDescription">A label representing a performed operation.</param>
        /// <param name="callback">A callback to process operation result.</param>
        /// <param name="token">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task StartAsync(
            this Progress progress,
            HttpClient client,
            HttpRequestMessage request,
            string taskDescription,
            Func<Stream, Task>? callback = null,
            CancellationToken token = default)
        {
            return progress.WithHttp(client, request, taskDescription, callback)
                .StartAsync(token);
        }

        /// <summary>
        /// Creates <see cref="HttpProgress"/> to hold a operation payload.
        /// </summary>
        /// <param name="progress">A <see cref="Progress"/> display element.</param>
        /// <param name="client">A <see cref="HttpClient"/>.</param>
        /// <param name="requestMessage">A <see cref="HttpRequestMessage"/>.</param>
        /// <param name="description">A label for the task.</param>
        /// <param name="callback">A callback to process operation result.</param>
        /// <returns>A container to capture payload.</returns>
        public static HttpProgress WithHttp(
            this Progress progress,
            HttpClient client,
            HttpRequestMessage requestMessage,
            string description,
            Func<Stream, Task>? callback = null)
        {
            var ctx = new HttpProgress(progress);
            ctx.Contexts.Add(
                new HttpProgressContext()
                {
                    Client = client, RequestMessage = requestMessage, Description = description, Callback = callback,
                });
            return ctx;
        }

        /// <summary>
        /// Runs <see cref="Progress"/> based on <see cref="HttpProgressContext"/>.
        /// </summary>
        /// <param name="progressContext">A underlying <see cref="ProgressContext"/>.</param>
        /// <param name="httpProgressContext">A captured operations.</param>
        /// <param name="token"> A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        internal static Task RunHttpReporterAsync(
            ProgressContext progressContext,
            HttpProgressContext httpProgressContext,
            CancellationToken token)
        {
            var message = httpProgressContext.RequestMessage
                          ?? throw new ArgumentException(nameof(httpProgressContext.RequestMessage));
            var client = httpProgressContext.Client
                         ?? throw new ArgumentException(nameof(httpProgressContext.Client));
            var callback = httpProgressContext.Callback;
            return RunHttpReporterAsync(
                progressContext, client, message, httpProgressContext.Description ?? string.Empty, callback, token);
        }

        private static async Task RunHttpReporterAsync(
            ProgressContext context,
            HttpClient client,
            HttpRequestMessage request,
            string taskDescription,
            Func<Stream, Task>? callback,
            CancellationToken token)
        {
            using var message = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
            _ = message.EnsureSuccessStatusCode();
            double? contentLength = message.Content.Headers.ContentLength;
            var total = contentLength ?? -1;
            var progressTask = context.AddTask(
                taskDescription, new ProgressTaskSettings { MaxValue = total });
#if NET5_0
            await using var stream = await message.Content.ReadAsStreamAsync(token)
                .ConfigureAwait(false);
#else
            await using var stream = await message.Content.ReadAsStreamAsync()
                .ConfigureAwait(false);
#endif
            await using var destination = new MemoryStream();
            await stream.CopyToAsync(destination, 2048, progressTask, token)
                .ConfigureAwait(false);
            if (callback is not null)
            {
                await callback.Invoke(destination).ConfigureAwait(false);
            }
        }
    }
}
