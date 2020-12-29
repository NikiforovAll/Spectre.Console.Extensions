# Spectre.Console.Extensions

![GitHub Actions Status](https://github.com/nikiforovall/Spectre.Console.Extensions/workflows/Build/badge.svg?branch=main)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

[![GitHub Actions Build History](https://buildstats.info/github/chart/nikiforovall/Spectre.Console.Extensions?branch=main&includeBuildsFromPullRequest=false)](https://github.com/nikiforovall/Spectre.Console.Extensions/actions)

The goal of this project is to extend [Spectre.Console](https://github.com/spectresystems/spectre.console) plugins with some niche functionality.

* Progress with `IProgress` Adapter.
* Progress with automatic reporting for `HttpClient`.
* Table with `DataTable`.

Package                               | Version                                                                                                                                            | Description
--------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------
`Spectre.Console.Extensions.Progress` | [![Nuget](https://img.shields.io/nuget/v/Spectre.Console.Extensions.Progress.svg)](https://nuget.org/packages/Spectre.Console.Extensions.Progress) | IProgress adapter and HttpClient reporting.
`Spectre.Console.Extensions.Table`    | [![Nuget](https://img.shields.io/nuget/v/Spectre.Console.Extensions.Table.svg)](https://nuget.org/packages/Spectre.Console.Extensions.Table)       | DataTable and DataSet support.

## Spectre.Console.Extensions.Progress ![NuGet Badge](https://buildstats.info/nuget/Spectre.Console.Extensions.Progress)

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
    .WithHttp(http, request, description, DownloadCallback1)
    .WithHttp(http, request, description, DownloadCallback2)
    .StartAsync();

```

## Spectre.Console.Extensions.Table ![NuGet Badge](https://buildstats.info/nuget/Spectre.Console.Extensions.Table)

Display `System.Data.DataTable`.

```csharp
System.Data.DataTable dataTable = DataTableFactory();
var table = dataTable.FromDataTable().Border(TableBorder.Rounded);
AnsiConsole.Render();
```

## TODO

* Add xml-doc for existing public APIs.
* Add statiq docs
* Add unit tests.
* Fix warnings; consider to TreatWarningsAsErrors

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

For more details, please see:

```bash
.
|-- Samples
|   |-- DataSet
|   |-- DataTable
|   |-- Directory.Build.props
|   |-- http-progress
|   |-- iprogress
|   `-- iprogress-http-client-multiple-calls
...
```

## License

Copyright © Alexey Nikiforov.

Provided as-is under the MIT license. For more information see [LICENSE.md](./LICENSE.md).
