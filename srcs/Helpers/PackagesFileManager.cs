using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI;
using WinUniversalTool.Models;
using WinUniversalTool.WebViewer;

namespace WinUniversalTool
{
    public static class PackagesFileManager
    {
        static readonly CancellationTokenSource dummy = new CancellationTokenSource();
        static readonly CancellationToken dummyToken = dummy.Token;
        public static async Task<string> ExtractPackage(StorageFile package, CancellationToken cancellationToken = default(CancellationToken), IProgress<string> progress = null)
        {
            var result = "";
            bool useZipMethod = false;

            if (package != null)
            {
                if (package.Name.Equals("AppxManifest.xml"))
                {
                    result = package.Path;
                    return result;
                }
            }
            try
            {
                if (progress != null)
                {
                    progress.Report("Checking packages folder");
                }
                var arch = "x64";

                var wutPackages = await PickPackagesFolder();
                if (wutPackages != null)
                {
                    var packagesFolder = await wutPackages.CreateFolderAsync("WUTPackages", CreationCollisionOption.OpenIfExists);
                    if (packagesFolder != null)
                    {
                        var noteFile = await packagesFolder.CreateFolderAsync("__DONT_REMOVE_THIS_FOLDER__", CreationCollisionOption.OpenIfExists);
                        noteFile = null;

                        var fileExt = Path.GetExtension(package.Name);
                        Stream packageStream = null;
                        var stream = (await (package != null ? package : package).OpenReadAsync()).AsStream();
                        
                        if (progress != null)
                        {
                            progress.Report("Verifying package");
                        }
                        switch (fileExt.ToLower())
                        {
                            case ".appxbundle":
                            case ".msixbundle":
                                ZipArchive zip = new ZipArchive(stream);
                                foreach (var item in zip.Entries)
                                {
                                    bool pickupAny = false;
                                    try
                                    {
                                        var packages = zip.Entries.Where(el => el.Name.ToLower().Contains(".appx") || el.Name.ToLower().Contains(".msix"));
                                        pickupAny = packages.Count() == 1; // bundle contains only one package
                                    }
                                    catch (Exception ex)
                                    {
                                        Helpers.Logger(ex);
                                    }
                                    if (item.Name.ToLower().Contains(".appx") || item.Name.ToLower().Contains(".msix"))
                                    {
                                        if ((!item.Name.ToLower().Contains("_language-") && !item.Name.ToLower().Contains("_scale-")) || pickupAny)
                                        {
#if TARGET_ARM
                    arch = "arm";
#elif TARGET_ARM64
                    arch = "arm64";
#elif TARGET_X86
                    arch = "x86";
#elif TARGET_X64
                                            arch = "x64";
#endif
                                            if (item.Name.ToLower().Contains(arch) || item.Name.ToLower().Contains("_neutral") || item.Name.ToLower().Contains("anycpu") || pickupAny)
                                            {
                                                packageStream = item.Open();
                                                break;
                                            }
                                        }
                                    }
                                }
                                break;

                            case ".appx":
                            case ".msix":
                                packageStream = stream;
                                break;
                            case ".xap":
                                packageStream = stream;
                                useZipMethod = true;
                                break;
                        }
                        if (packageStream != null)
                        {
                            Package packageContents = null;

                            var identity = package.Name;
                            ZipArchive zip = null;
                            string extra = "";
                            if (useZipMethod)
                            {
                                //Could happen in case of XAP
                                try
                                {
                                    zip = new ZipArchive(packageStream);
                                    if (zip != null)
                                    {
                                        useZipMethod = true;
                                        var item = zip.GetEntry("AppxManifest.xml");
                                        if (item != null)
                                        {
                                            var manifest = XElement.Load(item.Open());
                                            identity = manifest.Element(manifest.Name.Namespace + "Identity").Attribute("Name").Value;
                                        }
                                        else
                                        {
                                            zip = null;
                                            extra = "\nPackage doesn't contain AppxManifest.xml";
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Helpers.Logger(ex);
                                }
                            }
                            else
                            {
                                packageContents = Package.Open(packageStream);
                            }

                            if (packageContents != null || zip !=null)
                            {
                                StorageFolder packageExtractTarget = null;

                                if (useZipMethod)
                                {
                                    packageExtractTarget = await packagesFolder.CreateFolderAsync(identity, CreationCollisionOption.OpenIfExists);
                                    if (packageExtractTarget != null)
                                    {
                                        var total = zip.Entries.Count;
                                        var current = 1;
                                        bool isCanceled = false;
                                        bool isFailed = false;
                                        if (!isCanceled && !isFailed)
                                        {
                                            ParallelOptions parallelOptions = new ParallelOptions();
                                            parallelOptions.MaxDegreeOfParallelism = 5;
                                            parallelOptions.CancellationToken = cancellationToken;
                                            SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 5);
                                            await Task.Run(async () =>
                                            {
                                                //foreach (var part in parts)
                                                Parallel.ForEach(zip.Entries, parallelOptions, async (part) =>
                                                {
                                                    await semaphoreSlim.WaitAsync();
                                                    {
                                                        if (!isCanceled && !isFailed)
                                                        {
                                                            try
                                                            {
                                                                if (progress != null)
                                                                {
                                                                    progress.Report($"Extracting {current} of {total}");
                                                                }

                                                                var partStream = part.Open();
                                                                var partURI = Uri.UnescapeDataString(part.FullName);

                                                                if (partStream != null)
                                                                {
                                                                    var cleanedURI = partURI.TrimStart('/');
                                                                    var fileName = Path.GetFileName(cleanedURI);
                                                                    StorageFolder subFolder = null;
                                                                    StorageFile partFile = null;
                                                                    {
                                                                        if (cleanedURI.Contains("/"))
                                                                        {
                                                                            {
                                                                                var urlSub = Path.GetDirectoryName(cleanedURI);
                                                                                urlSub = urlSub.TrimEnd('/');

                                                                                subFolder = await packageExtractTarget.CreateFolderAsync(urlSub, CreationCollisionOption.OpenIfExists);
                                                                            }
                                                                        }

                                                                        if (!partURI.EndsWith("/"))
                                                                        {
                                                                            if (subFolder != null)
                                                                            {
                                                                                {
                                                                                    partFile = await subFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                {
                                                                                    partFile = await packageExtractTarget.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    if (partFile != null)
                                                                    {
                                                                        //IProgress<double> percentage = new Progress<double>(async value =>
                                                                        //{
                                                                        //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                                                                        //    {
                                                                        //        try
                                                                        //        {
                                                                        //            if (progress != null)
                                                                        //            {
                                                                        //                progress.Report($"Extracting {fileName} ({value}%)..\nState: {current} of {total}");
                                                                        //            }
                                                                        //        }
                                                                        //        catch (Exception xe)
                                                                        //        {

                                                                        //        }
                                                                        //    });
                                                                        //});
                                                                        using (var fileStream = await partFile.OpenStreamForWriteAsync())
                                                                        {
                                                                            await partStream.CopyToAsync(fileStream);
                                                                        }
                                                                        if (cancellationToken.IsCancellationRequested)
                                                                        {
                                                                            isCanceled = true;
                                                                            result = "Package deploy cancelled!";
                                                                            //break;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Helpers.Logger(new Exception($"Unable to create file for: {partURI}"));
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Helpers.Logger(new Exception($"Unable to get stream for: {partURI}"));
                                                                }
                                                                //current++;
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                isFailed = true;
                                                                Helpers.Logger(ex);
                                                                if (ex.Message.Contains("E_FAIL"))
                                                                {
                                                                    result = $"Failed, be sure the app is closed!\n{ex.Message}";
                                                                }
                                                                else
                                                                {
                                                                    result = ex.Message;
                                                                }
                                                                //break;
                                                            }
                                                        }
                                                    }
                                                    Interlocked.Increment(ref current);
                                                    semaphoreSlim.Release();
                                                });
                                                //extractState.SetResult(true);
                                            });

                                            TaskCompletionSource<bool> extractState = new TaskCompletionSource<bool>();

                                            await Task.Run(async () =>
                                            {
                                                try
                                                {
                                                    while (total >= current && !cancellationToken.IsCancellationRequested)
                                                    {
                                                        await Task.Delay(100);
                                                    }
                                                    extractState.SetResult(true);
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            });
                                            await extractState.Task;

                                            if (cancellationToken.IsCancellationRequested)
                                            {
                                                isCanceled = true;
                                                result = "Package deploy cancelled!";
                                            }

                                            if (!isCanceled && !isFailed)
                                            {
                                                StorageFile testManifest = null;
                                                try
                                                {
                                                    testManifest = (StorageFile)await packageExtractTarget.TryGetItemAsync("AppxManifest.xml");
                                                }
                                                catch
                                                {

                                                }
                                                if (testManifest != null)
                                                {
                                                    result = testManifest.Path;

                                                    try
                                                    {
                                                        var clean = new string[] { "AppxMetadata", "AppxBlockMap.xml", "AppxSignature.p7x", "[Content_Types].xml" };
                                                        foreach (var c in clean)
                                                        {
                                                            IStorageItem c1 = null;
                                                            try
                                                            {
                                                                c1 = await packageExtractTarget.TryGetItemAsync(c);
                                                            }
                                                            catch
                                                            {

                                                            }
                                                            if (c1 != null)
                                                            {
                                                                await c1.DeleteAsync();
                                                            }
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        Helpers.Logger(ex);
                                                    }
                                                    testManifest = null;
                                                }
                                                else
                                                {
                                                    result = "Unable to locate AppxManifest.xml";
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        result = $"Cannot get or create package folder\n{identity}";
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        var manifestPart = packageContents.GetPart(new Uri("/AppxManifest.xml", UriKind.Relative));
                                        var manifest = XElement.Load(manifestPart.GetStream());
                                        identity = manifest.Element(manifest.Name.Namespace + "Identity").Attribute("Name").Value;
                                    }
                                    catch (Exception ex)
                                    {
                                        Helpers.Logger(ex);
                                    }

                                    packageExtractTarget = await packagesFolder.CreateFolderAsync(identity, CreationCollisionOption.OpenIfExists);
                                    if (packageExtractTarget != null)
                                    {
                                        var parts = packageContents.GetParts();
                                        var total = parts.Count();
                                        var current = 1;
                                        bool isCanceled = false;
                                        bool isFailed = false;
                                        ParallelOptions parallelOptions = new ParallelOptions();
                                        parallelOptions.MaxDegreeOfParallelism = 5;
                                        parallelOptions.CancellationToken = cancellationToken;
                                        SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 5);
                                        await Task.Run(async () =>
                                        {
                                            //foreach (var part in parts)
                                            Parallel.ForEach(parts, parallelOptions, async (part) =>
                                            {
                                                await semaphoreSlim.WaitAsync();
                                                {
                                                    if (!isCanceled && !isFailed)
                                                    {
                                                        try
                                                        {
                                                            if (progress != null)
                                                            {
                                                                progress.Report($"Extracting {current} of {total}");
                                                            }

                                                            var partStream = part.GetStream();
                                                            var partURI = Uri.UnescapeDataString(part.Uri.ToString());

                                                            if (partStream != null)
                                                            {

                                                                var cleanedURI = partURI.TrimStart('/');
                                                                var fileName = Path.GetFileName(cleanedURI);
                                                                StorageFolder subFolder = null;
                                                                StorageFile partFile = null;
                                                                {
                                                                    if (cleanedURI.Contains("/"))
                                                                    {
                                                                        {
                                                                            var urlSub = Path.GetDirectoryName(cleanedURI);
                                                                            subFolder = await packageExtractTarget.CreateFolderAsync(urlSub, CreationCollisionOption.OpenIfExists);
                                                                        }
                                                                    }

                                                                    if (subFolder != null)
                                                                    {
                                                                        {
                                                                            partFile = await subFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        {
                                                                            partFile = await packageExtractTarget.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                                                                        }
                                                                    }
                                                                }
                                                                if (partFile != null)
                                                                {
                                                                    //IProgress<double> percentage = new Progress<double>(async value =>
                                                                    //{
                                                                    //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                                                                    //    {
                                                                    //        try
                                                                    //        {
                                                                    //            if (progress != null)
                                                                    //            {
                                                                    //                progress.Report($"Extracting {fileName} ({value}%)..\nState: {current} of {total}");
                                                                    //            }
                                                                    //        }
                                                                    //        catch (Exception xe)
                                                                    //        {

                                                                    //        }
                                                                    //    });
                                                                    //});
                                                                    using (var fileStream = await partFile.OpenStreamForWriteAsync())
                                                                    {
                                                                        await partStream.CopyToAsync(fileStream);
                                                                    }
                                                                    if (cancellationToken.IsCancellationRequested)
                                                                    {
                                                                        isCanceled = true;
                                                                        result = "Package deploy cancelled!";
                                                                        //break;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Helpers.Logger(new Exception($"Unable to create file for: {partURI}"));
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Helpers.Logger(new Exception($"Unable to get stream for: {partURI}"));
                                                            }
                                                            //current++;
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            isFailed = true;
                                                            Helpers.Logger(ex);
                                                            if (ex.Message.Contains("E_FAIL"))
                                                            {
                                                                result = $"Failed, be sure the app is closed!\n{ex.Message}";
                                                            }
                                                            else
                                                            {
                                                                result = ex.Message;
                                                            }
                                                            //break;
                                                        }
                                                    }
                                                }
                                                Interlocked.Increment(ref current);
                                                semaphoreSlim.Release();
                                            });
                                            //extractState.SetResult(true);
                                        });

                                        TaskCompletionSource<bool> extractState = new TaskCompletionSource<bool>();

                                        await Task.Run(async () =>
                                        {
                                            try
                                            {
                                                while (total >= current && !cancellationToken.IsCancellationRequested)
                                                {
                                                    await Task.Delay(100);
                                                }
                                                extractState.SetResult(true);
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        });
                                        await extractState.Task;

                                        if (cancellationToken.IsCancellationRequested)
                                        {
                                            isCanceled = true;
                                            result = "Package deploy cancelled!";
                                        }

                                        if (!isCanceled && !isFailed)
                                        {
                                            StorageFile testManifest = null;
                                            try
                                            {
                                                testManifest = (StorageFile)await packageExtractTarget.TryGetItemAsync("AppxManifest.xml");
                                            }
                                            catch
                                            {

                                            }
                                            if (testManifest == null)
                                            {
                                                try
                                                {
                                                    testManifest = (StorageFile)await packageExtractTarget.TryGetItemAsync("WMAppManifest.xml");
                                                }
                                                catch
                                                {

                                                }
                                            }
                                            if (testManifest != null)
                                            {
                                                result = testManifest.Path;

                                                try
                                                {
                                                    var clean = new string[] { "AppxMetadata", "AppxBlockMap.xml", "AppxSignature.p7x", "[Content_Types].xml" };
                                                    foreach (var c in clean)
                                                    {
                                                        IStorageItem c1 = null;
                                                        try
                                                        {
                                                            c1 = await packageExtractTarget.TryGetItemAsync(c);
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        if (c1 != null)
                                                        {
                                                            await c1.DeleteAsync();
                                                        }
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Helpers.Logger(ex);
                                                }
                                                testManifest = null;
                                            }
                                            else
                                            {
                                                result = "Unable to locate AppxManifest.xml";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        result = $"Cannot get or create package folder\n{identity}";
                                    }
                                }
                            }
                            else
                            {
                                result = $"Cannot get stream from package{extra}";
                            }
                        }
                        else
                        {
                            result = $"Cannot get package stream!\nNo packages found for {arch}!";
                        }
                    }
                    else
                    {
                        result = "Cannot create packages folder at downloads!";
                    }
                }
                else
                {
                    result = "Please setup WUTPackages folder first!";
                }
            }
            catch (Exception ex)
            {
                Helpers.Logger(ex);
                if (ex.Message.Contains("E_FAIL"))
                {
                    result = $"Failed, be sure the app is closed!\n{ex.Message}";
                }
                else
                {
                    result = ex.Message;
                }
            }
            await Task.Delay(100);
            return result;
        }
        public static async Task<StorageFolder> PickPackagesFolder(bool force = false)
        {
            StorageFolder wutPackages = await GetFolderForKey();
            try
            {
                if (wutPackages == null || force)
                {
                    Helpers.PlayNotificationSoundDirect("downloaded.mp3");
                    var DialogTitle3 = "WUTPackages";
                    var DialogMessage3 = $"Please setup WUTPackages folder:\n- Choose any location you want\n- External storage not supported\n\nImportant: don't skip this step, register packages cannot be done without this folder";
                    string[] DialogButtons3 = new string[] { $"Choose", "Cancel" };
                    if (wutPackages != null)
                    {
                        DialogMessage3 = $"WUTPackages folder:\n{wutPackages.Path}\n\n- Choose any location you want\n- External storage not supported\n\nNote: This location related to packages register feature only not normal installation";
                        DialogButtons3 = new string[] { $"Change", "Cancel" };
                    }
                    var QuestionDialog = Helpers.CreateDialog(DialogTitle3, DialogMessage3, DialogButtons3);
                    var QuestionResult = await QuestionDialog.ShowAsync2();
                    if (Helpers.DialogResultCheck(QuestionDialog, 2))
                    {
                        var folderPicker = new FolderPicker();
                        folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
                        folderPicker.FileTypeFilter.Add("*");

                        wutPackages = await folderPicker.PickSingleFolderAsync();
                        if(wutPackages != null)
                        {
                            RememberFolder(wutPackages);
                            Helpers.PlayNotificationSoundDirect("update.mp3");
                            LocalNotificationData localNotificationData = new LocalNotificationData();
                            localNotificationData.icon = SegoeMDL2Assets.Package;
                            localNotificationData.type = Colors.Green;
                            localNotificationData.time = 3;
                            localNotificationData.message = $"WUTPackages set to: {wutPackages.Path}";
                            Helpers.pushLocalNotification(null, localNotificationData);
                        }
                    }
                }
            }
            catch(Exception x)
            {
                _=Helpers.ShowCatchedErrorAsync(x);
            }

            return wutPackages;
        }

        public static string RememberFolder(StorageFolder folder)
        {
            string token = Guid.NewGuid().ToString();
            StorageApplicationPermissions.FutureAccessList.AddOrReplace(token, folder);
            Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("WUTPackages", token);
            return token;
        }
        public static async Task<StorageFolder> GetFolderForKey(string key = "WUTPackages")
        {
            StorageFolder wutPackages = null;
            try
            {
                string token = Plugin.Settings.CrossSettings.Current.GetValueOrDefault(key, "");
                if (token!=null && token.Length > 0 && StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
                {
                    wutPackages = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
                }
            }catch(Exception ex)
            {
                Helpers.Logger(ex);
            }
            return wutPackages;
        }
    }
}
