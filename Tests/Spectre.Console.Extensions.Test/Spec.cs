namespace Spectre.Console.Extensions.Test;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Extensions.Progress;
using Spectre.Console.Testing;
using Xunit;

public class Spec
{
    private readonly TestConsole testAnsiConsole;

    public Spec()
    {
        this.testAnsiConsole = new TestConsole();
        this.testAnsiConsole.Profile.Width = 10;
        this.testAnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.TrueColor;
        this.testAnsiConsole.Profile.Capabilities.Ansi = true;
        this.testAnsiConsole.Profile.Capabilities.Interactive = true;
    }

    [Fact]
    public async Task Progress_OneBarExtensionFromExtension_ReportIProgressAmountOnce()
    {
        const string taskName = "task1";
        const double amountToReport = 10d;
        ProgressTask? capturedProgressTask = default;
        var progress = new Progress(this.testAnsiConsole)
            .Columns(new ProgressColumn[] { new ProgressBarColumn() })
            .AutoRefresh(false)
            .AutoClear(true);
        await progress.StartAsync(ctx => capturedProgressTask = ctx.AddTask(taskName), Reporter);

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
        await progress
            .WithHttp(httpClient, request, string.Empty, (stream) => Task.CompletedTask)
            .StartAsync();
    }
}
