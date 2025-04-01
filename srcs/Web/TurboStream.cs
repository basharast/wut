using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinUniversalTool.Models;

namespace WinUniversalTool.WebViewer
{
    public class TurboStream
    {
        Windows.Web.Http.HttpResponseMessage httpResponse;
        private IProgress<double> progress;
        private long totalSize;
        private CancellationToken cancellationToken;
        private int bufferSize;

        public TurboStream(Windows.Web.Http.HttpResponseMessage httpResponse, IProgress<double> progress, CancellationToken cancellationToken)
        {
            this.httpResponse = httpResponse;
            this.progress = progress;
            try
            {
                totalSize = (long)httpResponse.Content.Headers.ContentLength.GetValueOrDefault();
            }
            catch (Exception e)
            {
                totalSize = 0;
            }
            this.bufferSize = Helpers.GetBufferSize(totalSize, false);
            this.cancellationToken = cancellationToken;
        }

        public async Task<string> FetchStream()
        {
            var lines = "";
            using (Stream stream = (await httpResponse.Content.ReadAsInputStreamAsync()).AsStreamForRead())
            {
                string line;
    
                using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
                    while ((line = await streamReader.ReadLineAsync()) != null)
                    {
                        lines += line;
                        if (progress != null)
                        {
                            try
                            {
                                var currentRead = Encoding.UTF8.GetByteCount(lines);
                                progress.Report((currentRead * 1d) / (totalSize * 1d) * 100);
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                
            }
            
            if (progress != null)
            {
                progress.Report(100);
            }
            return lines;
        }
    }

    public static class StreamExtensions3
    {
        public static async Task CopyToAsync3(this Stream source, Stream destination, IProgress<double> progress, CancellationToken cancellationToken = default(CancellationToken), long totalSize = 0, int bufferSize = 0x1000)
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
