namespace Spectre.Console.Extensions.Progress
{
    using System.Collections.Generic;
    using Progress = Spectre.Console.Progress;

    /// <summary>
    /// Container class to capture <see cref="HttpProgressContext"/>.
    /// </summary>
    public class HttpProgress
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpProgress"/> class.
        /// </summary>
        /// <param name="progress"><see cref="Progress"/> instance.</param>
        public HttpProgress(Progress progress) => this.Progress = progress;

        /// <summary>
        /// Gets an underplaying instance of <see cref="Progress"/>.
        /// </summary>
        internal Progress Progress { get; }

        /// <summary>
        /// Gets or sets captured requests.
        /// </summary>
        internal IList<HttpProgressContext> Contexts { get; set; } = new List<HttpProgressContext>();
    }
}
