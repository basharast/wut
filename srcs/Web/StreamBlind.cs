using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinUniversalTool.Models;

namespace WinUniversalTool.WebViewer
{
    public class StreamBlind : Stream
    {
        private readonly Stream baseStream;
        private readonly CancellationToken cancellationToken;
        private readonly IProgress<double> progress;

        public StreamBlind(Stream baseStream, IProgress<double> progress, CancellationToken cancellationToken)
        {
            this.baseStream = baseStream;
            this.cancellationToken = cancellationToken;
            this.progress = progress;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return baseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public async Task CopyToAsyncNew(Stream destination)
        {
            await baseStream.CopyToAsync2(destination, progress, cancellationToken);
        }

        public override bool CanRead => baseStream.CanRead;

        public override bool CanSeek => baseStream.CanSeek;

        public override bool CanWrite => baseStream.CanWrite;

        public override long Length => baseStream.Length;

        public override long Position { get => baseStream.Position; set => baseStream.Position = value; }

        public override void Flush()
        {
            baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }
    }

    public static class StreamExtensions2
    {
        public static async Task CopyToAsync2(this Stream source, Stream destination, IProgress<double> progress, CancellationToken cancellationToken = default(CancellationToken), int bufferSize = 0x1000)
        {
            using (destination)
            {
                using (source)
                {
                    var totalRead = 0L;
                    var buffer = new byte[bufferSize];
                    var isMoreDataToRead = true;

                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var read =  source.Read(buffer, 0, buffer.Length);

                        if (read == 0)
                        {
                            isMoreDataToRead = false;
                        }
                        else
                        {
                            try
                            {
                                // Write data on disk.
                                 destination.Write(buffer, 0, read);
                            }
                            catch (Exception e)
                            {
                               _=Helpers.ShowCatchedErrorAsync(e);
                                throw e;
                            }
                            totalRead += read;

                            if (progress != null)
                            {
                                await Task.Delay(15);
                                progress.Report(totalRead);
                            }
                        }
                    } while (isMoreDataToRead);
                }
            }
        }
    }
}
