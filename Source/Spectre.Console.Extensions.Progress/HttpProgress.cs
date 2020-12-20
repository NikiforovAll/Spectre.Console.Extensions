namespace Spectre.Console.Extensions.Progress
{
    using System.Collections.Generic;
    using Progress = Spectre.Console.Progress;

    public class HttpProgress
    {
        internal Progress Progress { get; }

        public HttpProgress(Progress progress) => this.Progress = progress;

        internal IList<HttpProgressContext> Contexts { get; set; } = new List<HttpProgressContext>();
    }
}
