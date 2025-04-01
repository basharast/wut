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
using Windows.System;

namespace Commander.Compiler
{
    public class CFolder
    {
        public StorageFolder StorageFolder;
        public string FolderID;
        public string[] Extensions;

        public CFolder(string folderID, string extensions)
        {
            FolderID = folderID;
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
        public CFolder(string folderID, StorageFolder storageFolder, string extensions = "*", bool append = false)
        {
            FolderID = folderID;
            StorageFolder = storageFolder;
            Extensions = new string[] { extensions };
            try
            {
                Extensions = extensions.ToString().TrimStart('|').Split('|');
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
            FolderPicker selectWpFolder = new FolderPicker();
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            foreach (var ExtensionItem in Extensions)
            {
                folderPicker.FileTypeFilter.Add(ExtensionItem);
            }
            StorageFolder = await folderPicker.PickSingleFolderAsync();

            if (append)
            {
                Append();
            }
        }

        public async Task<CFolder> CreateFolder(string folderID, string folderName, bool append = true)
        {
            var NewFolder = await StorageFolder.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting);
            CFolder cFolder = new CFolder(folderID, NewFolder, "*", append);
            return cFolder;
        }

        public async Task<CFile> CreateFile(string fileID, string fileName, bool append = true)
        {
            var NewFile = await StorageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            CFile cFile = new CFile(fileID, NewFile, append);
            return cFile;
        }

        public async Task<bool> Open()
        {
            var success = await Launcher.LaunchFolderAsync(StorageFolder);
            return success;
        }

        public async Task Delete()
        {
            await StorageFolder.DeleteAsync();
            CCache.FoldersCache.Remove(FolderID);
        }
        public void Append()
        {
            CFolder outPutTest = null;
            CCache.FoldersCache.TryGetValue(FolderID, out outPutTest);
            lock (CCache.FoldersCache)
            {
                if (outPutTest == null)
                {
                    CCache.FoldersCache.Add(FolderID, this);
                }
                else
                {
                    CCache.FoldersCache[FolderID] = this;
                }
            }
        }
    }
}
