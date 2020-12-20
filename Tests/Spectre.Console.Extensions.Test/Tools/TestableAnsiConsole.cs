namespace Spectre.Console.Extensions.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Spectre.Console.Rendering;

    public sealed class TestableAnsiConsole : IDisposable, IAnsiConsole
    {
        private readonly StringWriter writer;
        private readonly IAnsiConsole console;

        public TestableAnsiConsole(
            ColorSystem system,
            AnsiSupport ansi = AnsiSupport.Yes,
            InteractionSupport interaction = InteractionSupport.Yes,
            int width = 80)
        {
            this.writer = new StringWriter();
            this.console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = ansi,
                ColorSystem = (ColorSystemSupport)system,
                Interactive = interaction,
                Out = this.writer,
            });

            this.Width = width;
            this.Input = new TestableConsoleInput();
        }

        public string Output => this.writer.ToString();

        public Capabilities Capabilities => this.console.Capabilities;

        public Encoding Encoding => this.console.Encoding;

        public int Width { get; }

        public int Height => this.console.Height;

        public IAnsiConsoleCursor Cursor => this.console.Cursor;

        public TestableConsoleInput Input { get; }

        public RenderPipeline Pipeline => this.console.Pipeline;

        IAnsiConsoleInput IAnsiConsole.Input => this.Input;

        public void Dispose()
        {
            this.writer?.Dispose();
        }

        public void Clear(bool home)
        {
            this.console.Clear(home);
        }

        public void Write(IEnumerable<Segment> segments)
        {
            if (segments is null)
            {
                return;
            }

            foreach (var segment in segments)
            {
                this.console.Write(segment);
            }
        }
    }
}
