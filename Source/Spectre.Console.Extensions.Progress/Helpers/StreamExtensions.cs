namespace Spectre.Console.Extensions.Progress.Helpers
{
    using System;
    using System.IO;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A stream extensions to enable progress functionality.
    /// </summary>
    internal static class StreamExtensions
    {
        /// <summary>
        /// Copy source <see cref="Stream"/> to destination and reported the number of bytes read to provided <paramref name="progress"/>.
        /// </summary>
        /// <param name="source">A source.</param>
        /// <param name="destination">A destination.</param>
        /// <param name="bufferSize">A buffer size to perform reading and reporting.</param>
        /// <param name="progress">A progress element to report progress.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">Throw when it is not possible to report or perform copying.</exception>
        public static async Task CopyToAsync(
            this Stream source,
            Stream destination,
            int bufferSize,
            ProgressTask? progress = null,
            CancellationToken cancellationToken = default)
        {
            Guard();
            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)
                .ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken)
                    .ConfigureAwait(false);
                progress?.Increment(bytesRead);
            }

            void Guard()
            {
                var exception = (source, destination, bufferSize) switch
                {
                    (null, _, _) => new ArgumentNullException(nameof(source)),
                    ({ CanRead: false }, _, _) => new ArgumentException("Has to be readable", nameof(source)),
                    (_, null, _) => new ArgumentException(null, nameof(destination)),
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
