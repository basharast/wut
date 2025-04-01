using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Editing;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.UI.Core;

namespace WinUniversalTool.AppInstaller
{
    class Images2Video
    {
        public static bool TerminateConvert = false;
        public static async Task Convert(StorageFolder ImagesFolder, StorageFolder SaveFolder, IProgress<double> iprogress = null)
        {
            //Video Settings
            string ImagesType = "png";
            int VideoFrameRate = 7;
            bool OrderFilesByName = false;
            string VideoFileName = $"{ImagesFolder.Name}.mp4";
            var BitRate = 3000;

            //These values will be overrided by the images size
            int VideoHeight = 1080;
            int VideoWidth = 1920;

            TerminateConvert = false;

            //Get All Image from folder
            var FolderFiles = await ImagesFolder.GetFilesAsync();
            List<StorageFile> ImageFiles = new List<StorageFile>();
            int ImagesCount = 0;
            foreach (var ImageItem in FolderFiles)
            {
                var FileExtention = Path.GetExtension(ImageItem.Name).Replace(".", "");
                if (ImagesType.Contains(FileExtention.ToLower()))
                {
                    try
                    {
                        ImageFiles.Add(ImageItem);
                        Report($"Reading Images {ImagesCount} of {FolderFiles.Count}");
                        ImagesCount++;
                    }
                    catch (Exception ex)
                    {
                        Report(ex);
                    }
                }
            }


            var ClipDuration = 250;
            
            if (ImageFiles.Count > 0)
            {
                //Get Images Size W/H
                try
                {
                    var testImage = ImageFiles[0];
                    Stream imagestream = await testImage.OpenStreamForReadAsync();
                    BitmapDecoder dec = await BitmapDecoder.CreateAsync(imagestream.AsRandomAccessStream());
                    VideoWidth = (int)dec.PixelWidth;
                    VideoHeight = (int)dec.PixelHeight;
                    try
                    {
                        imagestream.Dispose();
                        dec = null;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                catch (Exception ex)
                {
                    Report(ex);
                }

                ImagesCount = 1;
                ClipDuration = 1000 / VideoFrameRate;
                Report($"Clip Duration: {ClipDuration}");

                //Order files by name need correct names
                //File 00001, File 00002 ..etc <-Correct names
                //File 1, File 2 ..etc Wrong names
                if (OrderFilesByName)
                {
                    ImageFiles = ImageFiles.OrderBy(o => o.DisplayName).ToList();
                }
                else
                {
                    ImageFiles = ImageFiles.OrderBy(o => o.DateCreated).ToList();
                }

                var processTerminated = false;
                TaskCompletionSource<bool> AddingTask = new TaskCompletionSource<bool>();
                var composition = new MediaComposition();

                //Add images to clip
                await Task.Run(async () =>
                {
                    foreach (var ClipImage in ImageFiles)
                    {
                        try
                        {
                            if (TerminateConvert)
                            {
                                processTerminated = true;
                                break;
                            }
                            MediaClip mediaClip = await MediaClip.CreateFromImageFileAsync(ClipImage, TimeSpan.FromMilliseconds(ClipDuration));
                            composition.Clips.Add(mediaClip);
                            ImagesCount++;
                            Report($"Adding Clips {ImagesCount} of {ImageFiles.Count}");
                        }
                        catch (Exception ex)
                        {
                            Report(ex);
                        }
                    }
                    try
                    {
                        AddingTask.SetResult(true);
                    }
                    catch (Exception ex)
                    {

                    }
                });

                await AddingTask.Task;
                if (!processTerminated)
                {
                    if (composition.Clips.Count > 0)
                    {
                        var VideoOutputFile = await SaveFolder.CreateFileAsync(VideoFileName, CreationCollisionOption.GenerateUniqueName);
                        var CurrentDuration = composition.Duration;
                        var profile = MediaEncodingProfile.CreateMp4(VideoProfileBySize(VideoWidth, VideoHeight));

                        profile.Video.Bitrate = (uint)BitRate * 1000;
                        profile.Video.Width = (uint)VideoWidth;
                        profile.Video.Height = (uint)VideoHeight;

                        //In case of any wrong results disable the line below
                        profile.Video.FrameRate.Numerator = (uint)VideoFrameRate;

                        bool isCompositingInProgress = true;
                        var RenderState = composition.RenderToFileAsync(VideoOutputFile, MediaTrimmingPreference.Precise, profile);
                        
                        //Below will be called during the process to report the percentage
                        RenderState.Progress = new AsyncOperationProgressHandler<TranscodeFailureReason, double>(async (info, progress) =>
                        {
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                try
                                {
                                    Report($"Processing Video {Math.Round(progress)}%");
                                    if (iprogress != null)
                                    {
                                        iprogress.Report(progress);
                                    }
                                }
                                catch (Exception ex)
                                {

                                }
                            });
                        });

                        //Below will be called when the process complete
                        RenderState.Completed = new AsyncOperationWithProgressCompletedHandler<TranscodeFailureReason, double>(async (info, status) =>
                        {
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                try
                                {
                                    var results = info.GetResults();
                                    if (results != TranscodeFailureReason.None || status != AsyncStatus.Completed)
                                    {
                                        Report("Saving was unsuccessful");
                                        await Task.Delay(1000);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            //Delete file here if you want 
                                            //await ImagesFolder.DeleteAsync();
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                                finally
                                {
                                    // Update UI whether the operation succeeded or not
                                }
                                isCompositingInProgress = false;
                            });
                        });


                        //Below will monitor the terminate request
                        while (isCompositingInProgress)
                        {
                            if (TerminateConvert)
                            {
                                Report("Script terminated!");
                                try
                                {
                                    RenderState.Cancel();
                                    isCompositingInProgress = false;
                                    break;
                                }
                                catch (Exception e)
                                {

                                }
                            }
                            await Task.Delay(1000);
                        }
                    }
                }
                else
                {
                    Report("Convert terminated!");
                }
            }
        }
        public static void Report(string text)
        {

        }
        public static void Report(Exception ex)
        {

        }

        public static VideoEncodingQuality VideoProfileBySize(int width, int height)
        {
            var defaultProfile = VideoEncodingQuality.HD1080p;
            try
            {
                if ((width >= 7680 && height >= 4320) || (width >= 4320 && height >= 7680))
                {
                    defaultProfile = VideoEncodingQuality.Uhd4320p;
                }
                else
                if ((width >= 4096 && height >= 2160) || (width >= 2160 && height >= 4096))
                {
                    defaultProfile = VideoEncodingQuality.Uhd2160p;
                }
                else
                if ((width >= 3840 && height >= 2160) || (width >= 2160 && height >= 3840))
                {
                    defaultProfile = VideoEncodingQuality.Uhd2160p;
                }
                else
                if ((width >= 1920 && height >= 1080) || (width >= 1080 && height >= 1920))
                {
                    defaultProfile = VideoEncodingQuality.HD1080p;
                }
                else if ((width >= 1280 && height >= 720) || (width >= 720 && height >= 1280))
                {
                    defaultProfile = VideoEncodingQuality.HD720p;
                }
                else if ((width >= 768 && height >= 480) || (width >= 480 && height >= 768))
                {
                    defaultProfile = VideoEncodingQuality.Wvga;
                }
                else if ((width >= 640 && height >= 480) || (width >= 480 && height >= 640))
                {
                    defaultProfile = VideoEncodingQuality.Vga;
                }
                else if ((width >= 320 && height >= 240) || (width >= 240 && height >= 320))
                {
                    defaultProfile = VideoEncodingQuality.Qvga;
                }
            }
            catch (Exception ex)
            {

            }

            return defaultProfile;
        }
    }
}
