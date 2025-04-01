/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Compiler;
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
    public class CFile
    {
        public StorageFile StorageFile;
        public string FileID;
        public string[] Extensions;

        public CFile(string fileID, string extensions)
        {
            FileID = fileID;
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

        public CFile(string fileID, StorageFile storageFile, bool append = false)
        {
            FileID = fileID;
            Extensions = new string[] { Path.GetExtension(storageFile.Name) };
            StorageFile = storageFile;

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
            StorageFile = await filePicker.PickSingleFileAsync();
            if (append)
            {
                Append();
            }
        }

        public async Task<bool> Open()
        {
            var success = await Windows.System.Launcher.LaunchFileAsync(StorageFile);
            return success;
        }

        public async Task Delete()
        {
            await StorageFile.DeleteAsync();
        }

        public void Append()
        {
            CFile outPutTest = null;
            CCache.FilesCache.TryGetValue(FileID, out outPutTest);
            lock (CCache.FilesCache)
            {
                if (outPutTest == null)
                {
                    CCache.FilesCache.Add(FileID, this);
                }
                else
                {
                    CCache.FilesCache[FileID] = this;
                }
            }
        }
    }
}
