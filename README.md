# Spectre.Console.Extensions

[![GitHub Actions Status](https://github.com/nikiforovall/Spectre.Console.Extensions/workflows/Build/badge.svg?branch=main)](https://github.com/nikiforovall/Spectre.Console.Extensions/actions)

[![GitHub Actions Build History](https://buildstats.info/github/chart/nikiforovall/Spectre.Console.Extensions?branch=main&includeBuildsFromPullRequest=false)](https://github.com/nikiforovall/Spectre.Console.Extensions/actions)

The goal of this project is to extend `Spectre.Console` with some niche plugins.

## Spectre.Console.Progress.Extensions

Use spectre spinner with standard `IProgress` interface.

```csharp
private static async Task RunSimpleExampleAsync()
{
    await BuildProgress().StartAsync(
        GenerateProgressTasks,
        (reporter) =>RunSpinnerWithIProgress(reporter, TimeSpan.FromMilliseconds(500)),
        (reporter) => RunSpinnerWithIProgress(reporter, TimeSpan.FromSeconds(1)));

    // Collection of tasks to execute, every task corresponds to 
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

Run progress for a given `HttpClient` and `HttpRequestMessage`. Result is provided as `System.IO.Stream`.

```csharp
await BuildProgress().StartAsync(
    httpClient,
    new HttpRequestMessage(HttpMethod.Get, url),
    taskDescription: url,
    callback: (stream) => SaveFileAsync);
```

For more details, please see:

```bash
samples/http-progress
samples/iprogress
samples/iprogress-http-client-multiple-calls
```

## Spectre.Console.Table.Extensions

TBD:

## TODO

* Add xml-doc for existing public APIs.
* Add `CancellationToken` to method based on `IProgerss`
* Finalize design for `Spectre.Console.Progress.Extensions`
* Consider to provide separate NuGet packages for plugins.
* Rename extras to `Spectre.Console.Extensions.<PluginName>`
* Add DataTable extension.
* Consider to add samples to CI.
* Add unit tests.

## Examples

To see `Spectre.Console` in action, install the 
[dotnet-example](https://github.com/patriksvensson/dotnet-example)
global tool.

```bash
> dotnet tool restore
```

Now you can list available examples in this repository:

```bash
> dotnet example
```

And to run an example:

```bash
> dotnet example tables
```

## License

Copyright © Spectre Systems.

Spectre.Console is provided as-is under the MIT license. For more information see LICENSE.
