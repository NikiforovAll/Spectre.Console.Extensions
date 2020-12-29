namespace Spectre.Console.Extensions.Test
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Spectre.Console.Extensions.Progress;
    using Spectre.Console;
    using Xunit;

    public class Spec
    {
        private readonly TestableAnsiConsole testAnsiConsole;

        public Spec()
        {
            this.testAnsiConsole = new TestableAnsiConsole(ColorSystem.TrueColor, width: 10);
        }

        [Fact]
        public async Task Progress_OneBarExtensionFromExtension_ReportIProgressAmountOnce()
        {
            // Given
            const string taskName = "task1";
            const double amountToReport = 10d;
            ProgressTask? capturedProgressTask = default;
            var progress = new Progress(this.testAnsiConsole)
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

        [Fact(Skip = "Learn how to mock http")]
        public async Task HttpProgress_OneHttpRequestFromWithHttp_SuccessProgress()
        {
            var progress = new Progress(this.testAnsiConsole)
                .Columns(new ProgressColumn[] { new ProgressBarColumn() })
                .AutoRefresh(false)
                .AutoClear(true);
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "/");
            await progress.WithHttp(httpClient, request,"", (stream) => Task.CompletedTask).StartAsync();
        }

        // TODO: consider to mock http client https://gingter.org/2018/07/26/how-to-mock-httpclient-in-your-net-c-unit-tests/
    }
}
