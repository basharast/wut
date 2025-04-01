/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.IO;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinUniversalTool.AppInstaller;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinUniversalTool.Models
{
    public class ZipArchiveManager
    {
        #region private helper functions 
        double Extracted = 0;
        double TotalFiles = 0;
        public string unzipReport = "";
        public async Task<bool> UnZipFileHelper(StorageFile zipFile, StorageFolder destinationFolder, IProgress<double> progressCallback = null, CancellationTokenSource cancellationTokenSource = null, AppxInstaller appxInstaller = null, IProgress<long> sizeCallback = null, IProgress<string> progressText = null)
        {
            var extension = zipFile.FileType;
            Extracted = 0;
            TotalFiles = 0;
            unzipReport = "";
            if (zipFile == null || destinationFolder == null)
            {
                throw new ArgumentException("Destination or Zip file is null!");
            }
            Stream zipMemoryStream = await zipFile.OpenStreamForReadAsync();
            var FileType = Path.GetExtension(zipFile.Name).ToLower();
            
            {
                {
                        SharpCompress.Readers.ReaderOptions readerOptions = null;
                        if (Helpers.UnzipWithPassword)
                        {
                            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                            readerOptions = new SharpCompress.Readers.ReaderOptions();
                            Exception passwordError = null;
                            await CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, async () =>
                            {
                                try
                                {
                                    Helpers.PlayNotificationSoundDirect("downloaded.mp3");
                                    Helpers.UnzipWithPassword = false;
                                    var pass = await ShowInputDialog("Archive Password", "Enter Password", "");
                                    if (pass != null && pass.Length > 0)
                                    {
                                        readerOptions.Password = pass;
                                    }
                                    else
                                    {
                                        throw new Exception("This file required password to extract!");
                                    }
                                    taskCompletionSource.SetResult(true);
                                }
                                catch (Exception ex)
                                {
                                    passwordError = ex;
                                    try
                                    {
                                        taskCompletionSource.SetResult(true);
                                    }
                                    catch (Exception exx)
                                    {

                                    }
                                }
                            });
                            await taskCompletionSource.Task;
                            if (passwordError != null)
                            {
                                throw passwordError;
                            }
                        }

                        using (var zipArchive = ArchiveFactory.Open(zipMemoryStream, readerOptions))
                        {
                            // Unzip compressed file iteratively. 
                            try
                            {
                                TotalFiles = zipArchive.Entries.Where(item => !item.IsDirectory).Count();
                            }
                            catch (Exception ex)
                            {
                                TotalFiles = zipArchive.Entries.Count();
                            }
                            double Progress = 0;
                            var reader = zipArchive.ExtractAllEntries();
                            //foreach (var entry in zipArchive.Entries.Where(entry => !entry.IsDirectory))
                            if (Helpers.ShowDetailedExtract)
                            {
                                reader.EntryExtractionProgress += (sender, e) =>
                                {
                                    try
                                    {
                                        var entryProgress = e.ReaderProgress.PercentageReadExact;
                                        var sizeProgress = e.ReaderProgress.BytesTransferred.ToFileSize();
                                        _ = CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () =>
                                        {
                                            try
                                            {
                                                if (appxInstaller != null && progressText == null)
                                                {
                                                    appxInstaller.ChangeStateText($"Extracting {Extracted}/{TotalFiles} ({Math.Round(Progress)}%) \nFile: {Path.GetFileName(e.Item.Key)} \nProgress: ({Math.Round(entryProgress)}%) / {sizeProgress} of {e.Item.Size.ToFileSize()}");
                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                if (progressText != null)
                                                {
                                                    _ = CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () =>
                                                    {
                                                        progressText.Report($"Extracting {Extracted}/{TotalFiles} ({Math.Round(Progress)}%) \nFile: {Path.GetFileName(e.Item.Key)} \nProgress: ({Math.Round(entryProgress)}%) / {sizeProgress} of {e.Item.Size.ToFileSize()}");

                                                    });
                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                if (progressCallback != null)
                                                {
                                                    progressCallback.Report(entryProgress);
                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        });
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                };
                            }
                            while (reader.MoveToNextEntry())
                            {
                                try
                                {
                                    if (!reader.Entry.IsDirectory)
                                    {
                                        Helpers.UnzipInProgress = true;

                                        var filePath = reader.Entry.Key;
                                        var totalSize = reader.Entry.Size;

                                        Extracted++;
                                        try
                                        {
                                            Progress = (Extracted / TotalFiles) * 100;
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                        if (progressCallback != null)
                                        {
                                            _ = CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () =>
                                            {
                                                try
                                                {
                                                    progressCallback.Report(Progress);
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            });
                                        }


                                        if (appxInstaller != null && progressText == null)
                                        {
                                            _ = CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () =>
                                            {
                                                try
                                                {
                                                    appxInstaller.ChangeStateText($"Extracting {Extracted}/{TotalFiles} ({Math.Round(Progress)}%) \nFile: {Path.GetFileName(filePath)} \nProgress: Please wait..");
                                                }
                                                catch (Exception e)
                                                {

                                                }
                                            });
                                        }
                                        if (progressText != null)
                                        {
                                            _ = CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, () =>
                                            {
                                                try
                                                {
                                                    progressText.Report($"Extracting {Extracted}/{TotalFiles} ({Math.Round(Progress)}%) \nFile: {Path.GetFileName(filePath)} \nProgress: Please wait..");
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            });
                                        }
                                        try
                                        {
                                            await reader.WriteEntryToDirectory(destinationFolder, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true }, cancellationTokenSource);
                                        }
                                        catch (Exception ex)
                                        {
                                            unzipReport = $"Error: cannot extract {filePath}\n{ex.Message}";
                                            Helpers.Logger($"Error cannot extract:\n{filePath}\n{ex.Message}");
                                        }

                                        if (sizeCallback != null)
                                        {
                                            sizeCallback.Report(totalSize);
                                        }
                                        if (cancellationTokenSource.IsCancellationRequested)
                                        {
                                            Helpers.UnzipInProgress = false;
                                            try
                                            {
                                                reader.Cancel();
                                            }
                                            catch (Exception e)
                                            {

                                            }
                                            return false;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw e;
                                }
                            }
                            try
                            {
                                zipArchive.Dispose();
                            }
                            catch (Exception e)
                            {

                            }
                            try
                            {
                                zipMemoryStream.Dispose();
                            }
                            catch (Exception e)
                            {

                            }
                        }
                        //Helpers.CallGCCollect();
                        return true;
                }
            }
            return false;
        }


        /// <summary> 
        /// It checks if the specified path contains directory. 
        /// </summary> 
        /// <param name="entryPath">The specified path</param> 
        /// <returns></returns> 
        private bool IfPathContainDirectory(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                return false;
            }
            return entryPath.Contains("/");
        }
        /// <summary> 
        /// It checks if the specified folder exists. 
        /// </summary> 
        /// <param name="storageFolder">The container folder</param> 
        /// <param name="subFolderName">The sub folder name</param> 
        /// <returns></returns> 
        private async Task<bool> IfFolderExistsAsync(StorageFolder storageFolder, string subFolderName)
        {
            try
            {
                var testFolder = (StorageFolder)await storageFolder.TryGetItemAsync(subFolderName);
                return testFolder != null;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary> 
        /// Unzips ZipArchiveEntry asynchronously. 
        /// </summary> 
        /// <param name="entry">The entry which needs to be unzipped</param> 
        /// <param name="filePath">The entry's full name</param> 
        /// <param name="unzipFolder">The unzip folder</param> 
        /// <returns></returns> 
        private async Task<long> UnzipZipArchiveEntryAsync(SharpCompress.Readers.IReader reader, string filePath, StorageFolder unzipFolder)
        {
            long fileSize = 0;
            try
            {
                if (IfPathContainDirectory(filePath))
                {
                    // Create sub folder 
                    string subFolderName = Path.GetDirectoryName(filePath);
                    bool isSubFolderExist = await IfFolderExistsAsync(unzipFolder, subFolderName);
                    StorageFolder subFolder;
                    if (!isSubFolderExist)
                    {
                        // Create the sub folder. 
                        subFolder = await unzipFolder.CreateFolderAsync(subFolderName, CreationCollisionOption.ReplaceExisting);
                    }
                    else
                    {
                        // Just get the folder. 
                        subFolder = await unzipFolder.GetFolderAsync(subFolderName);
                    }
                    // All sub folders have been created. Just pass the file name to the Unzip function. 
                    string newFilePath = Path.GetFileName(filePath);
                    if (!string.IsNullOrEmpty(newFilePath))
                    {
                        // Unzip file iteratively. 
                        return await UnzipZipArchiveEntryAsync(reader, newFilePath, subFolder);
                    }
                }
                else
                {
                    // Read uncompressed contents 
                    using (Stream entryStream = reader.OpenEntryStream())
                    {
                        fileSize = reader.Entry.Size;
                        if (fileSize > 0)
                        {
                            // byte[] buffer = new byte[entry.Size];
                            //entryStream.Read(buffer, 0, buffer.Length);
                            // Create a file to store the contents 
                            StorageFile uncompressedFile = await unzipFolder.CreateFileAsync(Path.GetFileName(reader.Entry.Key), CreationCollisionOption.ReplaceExisting);
                            // Store the contents 
                            using (IRandomAccessStream uncompressedFileStream =
                            await uncompressedFile.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                using (Stream outstream = uncompressedFileStream.AsStreamForWrite())
                                {
                                    await entryStream.CopyToAsync(outstream);
                                    try
                                    {
                                        entryStream.Dispose();
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                    //await outstream.WriteAsync(buffer, 0, buffer.Length);
                                    //await outstream.FlushAsync();
                                }
                            }
                        }
                        else
                        {
                            StorageFile uncompressedFile = await unzipFolder.CreateFileAsync(Path.GetFileName(reader.Entry.Key), CreationCollisionOption.ReplaceExisting);
                        }
                    }
                }
            }
            catch (Exception ee)
            {

            }
            return fileSize;
        }
        #endregion

        private async Task<string> ShowInputDialog(string title, string message, string defaultText, bool defaultTextset = true)
        {
            int left = 0;
            int top = 3;
            int right = 0;
            int bottom = 5;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = message;
            textBlock.Margin = new Thickness(left, top, right, bottom);

            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            inputTextBox.PlaceholderText = defaultText;
            if (defaultTextset)
            {
                inputTextBox.Text = defaultText;
            }
            inputTextBox.Margin = new Thickness(left, top, right, bottom);


            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(inputTextBox);

            ContentDialog dialog = new ContentDialog();
            dialog.Content = stackPanel;
            dialog.Title = title;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "OK";

            dialog.SecondaryButtonText = "Cancel";
            Helpers.DialogInProgress = true;
            switch (Helpers.AppTheme)
            {
                case "System":
                    dialog.RequestedTheme = ElementTheme.Default;
                    break;

                case "Dark":
                    dialog.RequestedTheme = ElementTheme.Dark;
                    break;

                case "Light":
                    dialog.RequestedTheme = ElementTheme.Light;
                    break;
            }
            Helpers.ChangeDialogBackgroudn(dialog);
            var result = await dialog.ShowAsync();
            Helpers.DialogInProgress = false;
            if (result == ContentDialogResult.Primary)
            {
                return inputTextBox.Text;
            }
            return null;
        }
    }
}
