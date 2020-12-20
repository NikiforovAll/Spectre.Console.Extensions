namespace Spectre.Console.Extensions
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class StreamExtensions
    {
        public static async Task CopyToAsync(
            this Stream source,
            Stream destination,
            int bufferSize,
            // IProgress<double> progress = null,
            ProgressTask progress = null,
            CancellationToken cancellationToken = default)
        {
            Guard();
            byte[] buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                // progress?.Report(totalBytesRead);
                progress.Increment(bytesRead);
            }

            void Guard()
            {
                var exception = (source, destination, bufferSize) switch
                {
                    (null, _, _) => new ArgumentNullException(nameof(source)),
                    ({ CanRead: false }, _, _) => new ArgumentException("Has to be readable", nameof(source)),
                    (_, null, _) => new ArgumentException(nameof(destination)),
                    (_, { CanWrite: false }, _) => throw new ArgumentException("Has to be writable", nameof(destination)),
                    (_, _, < 0) => new ArgumentOutOfRangeException(nameof(bufferSize)),
                    _ => null
                };

                if (exception is not null)
                {
                    throw exception;
                }
            }
        }
    }
}
