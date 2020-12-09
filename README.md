# Spectre.Console.Extensions

[![GitHub Actions Status](https://github.com/nikiforovall/Spectre.Console.Extensions/workflows/Build/badge.svg?branch=main)](https://github.com/nikiforovall/Spectre.Console.Extensions/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/nikiforovall/Spectre.Console.Extensions?branch=main&includeBuildsFromPullRequest=false)](https://github.com/nikiforovall/Spectre.Console.Extensions/actions)

The goal of this project is to extend `Spectre.Console` with some niche plugins.

## Spectre.Console.Progress.Extensions

Extensions for `AnsiConsole.Progress`

### IProgress-based

Use spectre spinner with standard `IProgress` interface.

**Motivation**: To plug methods that accept `IProgress` so that reporting is declarative, familiar, convenient.

```csharp
private static async Task RunSimpleExampleAsync()
{
    await BuildProgress().StartAsync(
        GenerateProgressTasks,
        (reporter) => RunSpinnerWithIProgress(reporter, TimeSpan.FromMilliseconds(500)),
        (reporter) => RunSpinnerWithIProgress(reporter, TimeSpan.FromSeconds(1)));

    // Collection of tasks to execute,
    // every task corresponds to following delegates sequentially.
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
```

### HttpClient-based

Run progress for a given `HttpClient` and `HttpRequestMessage`. Result is provided as `System.IO.Stream`.

**Motivation**: It is quite a common task to download something and have a spinner for that.

```csharp
var message = new HttpRequestMessage(HttpMethod.Get, url);
var http = new HttpClient();
var description = "Downloading cats images";
await BuildProgress().StartAsync(http, message, taskDescription: description, DownloadCallback);

static async Task DownloadCallback(Stream stream) => {};

await BuildProgress()
    .WithHttp(http, request, description)
    .WithHttp(http, request, description)
    .StartAsync(DownloadCallback1, DownloadCallback2);

```

For more details, please see:

```bash
samples/http-progress
samples/iprogress
samples/iprogress-http-client-multiple-calls
```

## Spectre.Console.Table.Extensions

remember to not bring exact version of DataTables, use X.Y
Check guidelines for microsoft for requirements for nuget packages

TBD:

## TODO

* Add xml-doc for existing public APIs.
* Add `CancellationToken` to method based on `IProgerss`
* Finalize design for `Spectre.Console.Progress.Extensions`
* Consider to provide separate NuGet packages for plugins.
* Rename extras to `Spectre.Console.Extensions.<PluginName>`
* Add link to base lib and license reference
* Adjust package description
* Add DataTable extension.
* Consider to add samples to CI.
* Add statiq docs
* Consider to add Start overload for IProgress scenario
* Consider to remove IProgress from HttpProgress
* Consider to add Progress.WithHttpClient().StartAsync
  * aka fluent version
  * consider overload that allows to make multiple calls
* Add HttpProgress overload to support multiple simultaneous calls.
* Check Stream disposal for HttpProgress
* Add unit tests.

## Examples

To see `Spectre.Console` in action, install the [dotnet-example](https://github.com/patriksvensson/dotnet-example) global tool.

```bash
> dotnet tool restore
```

Now you can list available examples in this repository:

```bash
> dotnet example
```

And to run an example:

```bash
> dotnet example iprogress
```

## License

Copyright © Spectre Systems.

Spectre.Console is provided as-is under the MIT license. For more information see LICENSE.
