using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WinUniversalTool.Models;

namespace WinUniversalTool.WebViewer
{
    public class StreamProgress : Stream
    {
        private BrowseListModel browseListModel;
        private Stream baseStream;
        private IProgress<double> progress;
        private long totalSize;
        private CancellationToken cancellationToken;
        private long totalRead;
        private int bufferSize;
        private bool AudioExtract;

        public StreamProgress(BrowseListModel browseListModel, IProgress<double> progress, CancellationToken cancellationToken, bool AudioExtract = false)
        {
            this.browseListModel = browseListModel;
            this.progress = progress;
            this.totalSize = browseListModel.fileSizeLong;
            this.bufferSize = Helpers.GetBufferSize(totalSize, false);
            this.cancellationToken = cancellationToken;
            this.AudioExtract = AudioExtract;
            totalRead = 0L;
        }

        public async Task<Stream> FetchStream()
        {
            if (browseListModel.megaFileNode.DirectStream != null)
            {
                baseStream = browseListModel.megaFileNode.DirectStream;
            }
            else
            {
                var baseStreamTemp = await browseListModel.megaFileNode.responseMessage.Content.ReadAsInputStreamAsync();
                baseStream = baseStreamTemp.AsStreamForRead();
            }
            return this;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return baseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return baseStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public async Task CopyToAsyncNew(Stream destination)
        {
            await baseStream.CopyToAsync(destination, progress, cancellationToken, totalSize, bufferSize);
        }

        public override bool CanRead => baseStream.CanRead;

        public override bool CanSeek => baseStream.CanSeek;

        public override bool CanWrite => baseStream.CanRead;

        public override long Length => baseStream.Length;

        public override long Position { get => baseStream.Position; set => baseStream.Position = value; }

        public override void Flush()
        {
            baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = baseStream.Read(buffer, offset, count);
            ReportProgress(read);
            return read;
        }

        private void ReportProgress(int read)
        {
            totalRead += read;

            if (progress != null && totalSize > 0)
            {
                progress.Report((totalRead * 1d) / (totalSize * 1d) * 100);
            }
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

    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<double> progress, CancellationToken cancellationToken = default(CancellationToken), long totalSize = 0, int bufferSize = 0x1000)
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

                        var read = source.Read(buffer, 0, buffer.Length);

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
                                _ = Helpers.ShowCatchedErrorAsync(e);
                                throw e;
                            }
                            totalRead += read;

                            if (progress != null && totalSize > 0)
                            {
                                await Task.Delay(10);
                                progress.Report((totalRead * 1d) / (totalSize * 1d) * 100);
                            }
                        }
                    } while (isMoreDataToRead);
                }
            }
        }
    }
}
