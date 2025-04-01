/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinUniversalTool.Models
{
    public class Chunks
    {
        public int MaxChunks = 1;
        public List<ChunkItem> ChunkItems;
        List<Task> Tasks = new List<Task>();
        public double CurrentProgress
        {
            get
            {
                double TotalProgress = 0;
                try
                {
                    if (ChunkItems != null && ChunkItems.Count > 0)
                    {
                        if (MaxChunks > 1)
                        {
                            foreach (var ChunkItem in ChunkItems)
                            {
                                TotalProgress += ChunkItem.CurrentProgress;
                            }
                            TotalProgress = TotalProgress / ChunkItems.Count;
                        }
                        else
                        {
                            TotalProgress = ChunkItems.FirstOrDefault().CurrentProgress;
                        }
                    }
                }
                catch (Exception e)
                {
                    _ = Helpers.ShowCatchedErrorAsync(e);
                }
                return Math.Round(TotalProgress);
            }
        }
        public Chunks(Stream remoteStream, StorageFolder storageFolder, string fileName, long fileSize, ref EventHandler DownloadErrorCatched, bool streamCanSeek = false)
        {
            ChunkItems = new List<ChunkItem>();
            if (streamCanSeek)
            {
                MaxChunks = 6;
            }
            int ChunkSize = (int)Math.Round((fileSize / MaxChunks * 1.0)) + 1;

            for (int i = 0; i < MaxChunks; i++)
            {
                int ChunkStart = ChunkSize * i;
                ChunkItem chunkItem = new ChunkItem(i, MaxChunks, fileName, ChunkSize, ChunkStart, remoteStream, storageFolder, ref Tasks, ref DownloadErrorCatched);
                ChunkItems.Add(chunkItem);
            }
        }
        public async void StartDownload()
        {
            try
            {
                Task.WaitAll(Tasks.ToArray());
            }
            catch (Exception e)
            {
                _ = Helpers.ShowCatchedErrorAsync(e);
            }
        }
    }
}
