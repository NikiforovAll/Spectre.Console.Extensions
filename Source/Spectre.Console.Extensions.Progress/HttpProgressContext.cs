namespace Spectre.Console.Extensions.Progress
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal class HttpProgressContext
    {
        public HttpClient? Client { get; init; }

        public HttpRequestMessage? RequestMessage { get; init; }

        public string? Description { get; init; }

        public Func<Stream, Task>? Callback { get; init; }
    }
}
