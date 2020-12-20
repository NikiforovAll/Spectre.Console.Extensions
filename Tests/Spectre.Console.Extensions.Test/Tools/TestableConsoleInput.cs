namespace Spectre.Console.Extensions.Test
{
    using System;
    using System.Collections.Generic;

    public sealed class TestableConsoleInput : IAnsiConsoleInput
    {
        private readonly Queue<ConsoleKeyInfo> input;

        public TestableConsoleInput()
        {
            this.input = new Queue<ConsoleKeyInfo>();
        }

        public void PushText(string input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            foreach (var character in input)
            {
                this.PushCharacter(character);
            }

            this.PushKey(ConsoleKey.Enter);
        }

        public void PushCharacter(char character)
        {
            var control = char.IsUpper(character);
            this.input.Enqueue(new ConsoleKeyInfo(character, (ConsoleKey)character, false, false, control));
        }

        public void PushKey(ConsoleKey key)
        {
            this.input.Enqueue(new ConsoleKeyInfo((char)key, key, false, false, false));
        }

        public ConsoleKeyInfo ReadKey(bool intercept)
        {
            if (this.input.Count == 0)
            {
                throw new InvalidOperationException("No input available.");
            }

            return this.input.Dequeue();
        }
    }
}
