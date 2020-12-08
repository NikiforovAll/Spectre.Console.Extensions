namespace Spectre.Console.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

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
                var tasks = new Task[rendarableTasks.Length];
                for (var i = 0; i < tasks.Length; i++)
                {
                    ProgressTask rendarableTask = rendarableTasks[i];
                    var arg = new Progress<double>((delta) =>
                    {
                        rendarableTask.Increment(delta);
                    });
                    var taskResult = actions[i].Invoke(arg);
                    tasks[i] = taskResult;
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
                new ProgressTask[]
                {
                    taskFactory?.Invoke(ctx)
                        ?? throw new ArgumentException(nameof(StartAsync), nameof(taskFactory)),
                };
            return progress.StartAsync(NewFactory, action);
        }

        public static async Task StartAsync(
            this Progress progress,
            HttpClient client,
            HttpRequestMessage request,
            string taskDescription,
            Func<Stream, Task> callback,
            CancellationToken token = default)
        {
            using var message = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                token);
            _ = message.EnsureSuccessStatusCode();
            double? contentLength = message.Content.Headers.ContentLength;
            var total = contentLength ?? -1;

            await progress.StartAsync(
                (ctx) => ctx.AddTask(taskDescription,
                    new() { MaxValue = total }),
                async (reporter) =>
            {
                using var stream = await message.Content.ReadAsStreamAsync(token)
                    .ConfigureAwait(false);
                using var destination = new MemoryStream();
                await stream.CopyToAsync(destination, 2048, reporter, token);
                await callback.Invoke(destination).ConfigureAwait(false);
            });
        }
    }
}
