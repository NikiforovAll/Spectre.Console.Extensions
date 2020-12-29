namespace Spectre.Console.Extensions.Progress
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// A <see cref="HttpProgress"/> extension methods to configure reportable HTTP progress bar.
    /// </summary>
    internal class HttpProgressContext
    {
        /// <summary>
        /// Gets client.
        /// </summary>
        public HttpClient? Client { get; init; }

        /// <summary>
        /// Gets a request message.
        /// </summary>
        public HttpRequestMessage? RequestMessage { get; init; }

        /// <summary>
        /// Gets a description.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Gets an optional callback to operate on result.
        /// </summary>
        public Func<Stream, Task>? Callback { get; init; }
    }
}
