/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WinUniversalTool.MegaApiClientSource;
using WinUniversalTool.WebViewer;
using Windows.Storage;

namespace WinUniversalTool.Models
{
    public class ChunkItem
    {
        public string ChunkName;
        public long ChunkSize;
        public long ChunkStart;
        public Stream RemoteStream;
        public double CurrentProgress = 0;
        public string CurrentState;
        public StorageFile ChunkFile;

        public ChunkItem(int ChunkIndex, int MaxChunks, string chunkName, long chunkSize, long chunkStart, Stream remoteStream, StorageFolder storageFolder, ref List<Task> tasks, ref EventHandler DownloadErrorCatched)
        {
            ChunkName = chunkName;
            ChunkSize = chunkSize;
            ChunkStart = chunkStart;
            RemoteStream = remoteStream;
            tasks.Add(CreatTask(ChunkIndex, MaxChunks, chunkName, storageFolder, DownloadErrorCatched));
        }
        private Task CreatTask(int ChunkIndex, int MaxChunks, string chunkName, StorageFolder storageFolder, EventHandler DownloadErrorCatched)
        {
            return Task.Factory.StartNew(async () =>
            {

                StorageFolder tempFolder = null;
                string ChunkFileName = chunkName;
                if (MaxChunks > 1)
                {
                    try
                    {
                        tempFolder = (StorageFolder)await storageFolder.TryGetItemAsync("_temp");
                    }
                    catch
                    {

                    }
                    if (tempFolder == null)
                    {
                        await storageFolder.CreateFolderAsync("_temp");
                        try
                        {
                            tempFolder = (StorageFolder)await storageFolder.TryGetItemAsync("_temp");
                        }
                        catch
                        {

                        }
                    }
                    ChunkFileName = Path.GetFileNameWithoutExtension(chunkName);
                    try
                    {
                        ChunkFile = (StorageFile)await tempFolder.TryGetItemAsync($"{ChunkFileName}_{ChunkIndex}");
                    }
                    catch
                    {

                    }
                }
                else
                {
                    tempFolder = storageFolder;
                }

                if (ChunkFile != null)
                {
                    var ChunkProperties = await ChunkFile.GetBasicPropertiesAsync();
                    var fileSize = (long)ChunkProperties.Size;
                    if (fileSize >= ChunkSize)
                    {
                        CurrentProgress = 100;
                        return;
                    }
                    else if (RemoteStream.CanSeek)
                    {
                        ChunkStart += fileSize;
                    }
                    else
                    {
                        await ChunkFile.DeleteAsync();
                        ChunkFile = await tempFolder.CreateFileAsync(chunkName);
                    }
                }
                else
                {
                    ChunkFile = await tempFolder.CreateFileAsync(chunkName);
                }

                try
                {
                    Stream output = null;

                    if (RemoteStream.CanSeek)
                    {
                        RemoteStream.Position = ChunkStart;
                    }

                    using (RemoteStream)
                    {
                        using (var targetStream = await ChunkFile.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            output = targetStream.AsStreamForWrite();
                            if (RemoteStream.GetType() == typeof(StreamProgress))
                            {
                                await ((StreamProgress)RemoteStream).CopyToAsyncNew(output);
                            }
                            else if (RemoteStream.GetType() == typeof(ProgressionStream))
                            {
                                await ((ProgressionStream)RemoteStream).CopyToAsyncNew(output);
                            }
                            else if (RemoteStream.GetType() == typeof(StreamBlind))
                            {
                                await ((StreamBlind)RemoteStream).CopyToAsyncNew(output);
                            }
                            else
                            {
                                await RemoteStream.CopyToAsync(output);
                            }
                            output.Dispose();
                            CurrentProgress = 100;
                        }
                    }
                }
                catch (Exception e)
                {
                    DownloadErrorCatched.Invoke(e, EventArgs.Empty);
                    //await Helpers.ShowCatchedErrorAsync(e);
                }
            });
        }

    }

}
