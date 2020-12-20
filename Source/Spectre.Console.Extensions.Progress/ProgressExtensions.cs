namespace Spectre.Console.Extensions.Progress
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Progress = Spectre.Console.Progress;

    /// <summary>
    /// Spectre Progress extensions methods.
    /// </summary>
    public static class ProgressExtensions
    {
        public static Task StartAsync(
            this Progress progress,
            Func<ProgressContext, IEnumerable<ProgressTask>> taskFactory,
            params Func<IProgress<double>, Task>[] actions)
        {
            // TODO: throw exception based on length
            return progress.StartAsync(async (context) =>
            {
                var rendarableTasks = taskFactory?
                                          .Invoke(context)?.ToArray()
                                      ?? throw new ArgumentException(nameof(StartAsync));
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

        public static Task StartAsync(
            this Progress progress,
            Func<ProgressContext, ProgressTask> taskFactory,
            Func<IProgress<double>, Task> action)
        {
            ProgressTask[] NewFactory(ProgressContext ctx) =>
                new[]
                {
                    taskFactory?.Invoke(ctx)
                    ?? throw new ArgumentException(nameof(StartAsync), nameof(taskFactory)),
                };

            return progress.StartAsync(NewFactory, action);
        }

        public static Task StartAsync(
            this Progress progress,
            HttpClient client,
            HttpRequestMessage request,
            string taskDescription,
            Func<Stream, Task> callback = null,
            CancellationToken token = default)
        {
            return progress.WithHttp(client, request, taskDescription, callback).StartAsync(token);
        }

        internal static Task RunHttpReporterAsync(
            ProgressContext progressContext,
            HttpProgressContext httpProgressContext,
            CancellationToken token) =>
            RunHttpReporterAsync(
                progressContext,
                httpProgressContext.Client ?? throw new ArgumentException(nameof(httpProgressContext.Client)),
                httpProgressContext.RequestMessage ?? throw new ArgumentException(nameof(httpProgressContext.RequestMessage)),
                httpProgressContext.Description ?? string.Empty,
                httpProgressContext.Callback,
                token);

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
            await using var stream = await message.Content.ReadAsStreamAsync(token)
                .ConfigureAwait(false);
            await using var destination = new MemoryStream();
            await stream.CopyToAsync(destination, 2048, progressTask, token)
                .ConfigureAwait(false);
            if (callback is not null)
            {
                await callback.Invoke(destination).ConfigureAwait(false);
            }
        }

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
    }
}
