namespace Spectre.Console.Progress.Extensions.Test
{
    using System;
    using System.Threading.Tasks;
    using Spectre.Console;
    using Spectre.Console.Extensions;
    using Xunit;

    public class Spec
    {
        [Fact]
        public async Task Progress_OneBarExtensionFromExtenssion_ReportIProgressAmountOnce()
        {
            // Given
            var console = new TestableAnsiConsole(ColorSystem.TrueColor, width: 10);
            var taskName = "task1";
            var amountToReport = 10d;
            ProgressTask? capturedProgressTask = default;
            var progress = new Progress(console)
                .Columns(new[] { new ProgressBarColumn() })
                .AutoRefresh(false)
                .AutoClear(true);
            await progress.StartAsync(
                ctx => capturedProgressTask = ctx.AddTask(taskName), Reporter);

            Assert.Equal(amountToReport, capturedProgressTask!.Value);

            Task Reporter(IProgress<double> reporter)
            {
                reporter.Report(amountToReport);
                return Task.CompletedTask;
            }
        }

        // TODO: consider to mock http client https://gingter.org/2018/07/26/how-to-mock-httpclient-in-your-net-c-unit-tests/
    }
}
