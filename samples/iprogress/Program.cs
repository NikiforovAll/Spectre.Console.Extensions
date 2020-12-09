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
        public static async Task Main() => await RunSimpleExampleAsync();

        private static async Task RunSimpleExampleAsync()
        {
            await BuildProgress().StartAsync(
                GenerateProgressTasks,
                (reporter) =>
                    RunSpinnerWithIProgress(reporter, TimeSpan.FromMilliseconds(500)),
                (reporter) =>
                    RunSpinnerWithIProgress(reporter, TimeSpan.FromSeconds(1)));

            static IEnumerable<ProgressTask> GenerateProgressTasks(ProgressContext ctx)
            {
                yield return ctx.AddTask("Task1");
                yield return ctx.AddTask("Task2");
            }

            static async Task RunSpinnerWithIProgress(
                IProgress<double> reporter,
                TimeSpan delay)
            {
                var capacity = 100;
                var step = 10;
                while (capacity > 0)
                {
                    reporter.Report(step);
                    capacity -= step;
                    await Task.Delay(delay);
                }
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
    }
}
