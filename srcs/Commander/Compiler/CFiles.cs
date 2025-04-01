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
using Windows.Storage.Pickers;

namespace Commander.Compiler
{
    public class CFiles
    {
        public List<StorageFile> StorageFiles;
        public string FilesID;
        public string[] Extensions;

        public CFiles(string filesID, string extensions)
        {
            FilesID = filesID;
            Extensions = new string[] { extensions };
            try
            {
                if (extensions.Contains("|"))
                {
                    Extensions = extensions.ToString().TrimStart('|').Split('|');
                }
                else
                {
                    Extensions = new string[] { extensions.ToString().Trim() };
                }
            }
            catch (Exception e)
            {

            }
        }
        public CFiles(string filesID, string extensions, List<StorageFile> storageFiles, bool append = false)
        {
            FilesID = filesID;
            Extensions = new string[] { extensions };
            StorageFiles = storageFiles;
            try
            {
                if (extensions.Contains("|"))
                {
                    Extensions = extensions.ToString().TrimStart('|').Split('|');
                }
                else
                {
                    Extensions = new string[] { extensions.ToString().Trim() };
                }
            }
            catch (Exception e)
            {

            }
            if (append)
            {
                Append();
            }
        }

        public async Task Select(bool append = false)
        {
            var filePicker = new FileOpenPicker();
            filePicker.SuggestedStartLocation = PickerLocationId.Downloads;
            foreach (var ExtensionItem in Extensions)
            {
                filePicker.FileTypeFilter.Add(ExtensionItem);
            }
            StorageFiles = (await filePicker.PickMultipleFilesAsync()).ToList();

            if (append)
            {
                Append();
            }
        }

        public async Task Open()
        {
            foreach (var StorageFileItem in StorageFiles)
            {
                var success = await Windows.System.Launcher.LaunchFileAsync(StorageFileItem);

            }
        }

        public async Task Delete()
        {
            foreach (var StorageFileItem in StorageFiles)
            {
                await StorageFileItem.DeleteAsync();
            }
        }

        public void Append()
        {
            CFiles outPutTest = null;
            CCache.MFilesCache.TryGetValue(FilesID, out outPutTest);
            lock (CCache.MFilesCache)
            {
                if (outPutTest == null)
                {
                    CCache.MFilesCache.Add(FilesID, this);
                }
                else
                {
                    CCache.MFilesCache[FilesID] = this;
                }
            }
        }
    }
}
