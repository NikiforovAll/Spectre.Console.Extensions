# Spectre.Console.Extensions

[![GitHub Actions Status](https://github.com/nikiforovall/Spectre.Console.Extensions/workflows/Build/badge.svg?branch=main)](https://github.com/nikiforovall/Spectre.Console.Extensions/actions)[![NuGet Badge](https://buildstats.info/nuget/Spectre.Console.Extensions)](https://www.nuget.org/packages/Spectre.Console.Extensions/)[![Microsoft.Extensions.Http.Polly on fuget.org](https://www.fuget.org/packages/Spectre.Console.Extensions/badge.svg)](https://www.fuget.org/packages/Spectre.Console.Extensions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/nikiforovall/Spectre.Console.Extensions?branch=main&includeBuildsFromPullRequest=false)](https://github.com/nikiforovall/Spectre.Console.Extensions/actions)

The goal of this project is to extend [Spectre.Console](https://github.com/spectresystems/spectre.console) plugins with some niche functionality.

* Progress with `IProgress` Adapter.
* Progress with automatic reporting for `HttpClient`.
* Table with `DataTable`.

## Spectre.Console.Extensions.Progress

Extensions for `AnsiConsole.Progress`

### IProgress Adapter

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

### Reporting for HttpClient

Run progress for a given `HttpClient` and `HttpRequestMessage`. Result is provided as `System.IO.Stream`.

**Motivation**: It is quite a common task to download something and have a spinner for that. Basically, you don't even wanna to bother with reporting in this case.

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

## Spectre.Console.Extensions.Table

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
* Add statiq docs
* Consider to add Start overload for IProgress scenario
* Consider to remove IProgress from HttpProgress
* Consider to add Progress.WithHttpClient().StartAsync
  * aka fluent version
  * consider overload that allows to make multiple calls
* Add HttpProgress overload to support multiple simultaneous calls.
* Check Stream disposal for HttpProgress
* Add unit tests.
* Change the .snk file
* Add .netstandard
* Add multiple nuget packages, as described here: <https://github.com/RehanSaeed/Serilog.Exceptions>
* Add tags to projects
* Fix warnings; consider to TreatWarningsAsErrors
* Add Fuget
* Add Nuget Labels
* Add New Layout for multiple NuGet packages, as for System.CommandLine;
* Add demos

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

Copyright © Alexey Nikiforov.

Provided as-is under the MIT license. For more information see LICENSE.
