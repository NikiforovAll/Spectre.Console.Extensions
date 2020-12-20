namespace Spectre.Console.Extensions.Test
{
    using System;
    using System.Threading.Tasks;
    using Spectre.Console;
    using Spectre.Console.Extensions;
    using Xunit;

    public class Spec
    {
        [Fact]
        public async Task Progress_OneBarExtensionFromExtenssion_ReportIProgressAmountOnceAsync()
        {
            // Given
            var console = new TestableAnsiConsole(ColorSystem.TrueColor, width: 10);
            const string taskName = "task1";
            const double amountToReport = 10d;
            ProgressTask? capturedProgressTask = default;
            var progress = new Progress(console)
                .Columns(new ProgressColumn[] { new ProgressBarColumn() })
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
