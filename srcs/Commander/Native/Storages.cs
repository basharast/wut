/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Commander.Native
{
    public static class Storages
    {
        public static async Task<CFolder> Folder(string folderID, string extensions)
        {
            CFolder cFolder = new CFolder(folderID, extensions);
            await cFolder.Select(true);
            return cFolder;
        }

        public static async Task<CFile> File(string fileID, string extensions)
        {
            CFile cFile = new CFile(fileID, extensions);
            await cFile.Select(true);
            return cFile;
        }

        public static async Task<CFiles> Files(string filesID, string extensions)
        {
            CFiles cFiles = new CFiles(filesID, extensions);
            await cFiles.Select(true);
            return cFiles;
        }

        public static async Task Pick(string targetID)
        {
            CFolder cFolder = null;
            CFile cFile = null;
            CFiles cFiles = null;
            if(CCache.FoldersCache.TryGetValue(targetID, out cFolder))
            {
                await cFolder.Select();
            }
            else if (CCache.FilesCache.TryGetValue(targetID, out cFile))
            {
                await cFile.Select();
            }
            else if (CCache.MFilesCache.TryGetValue(targetID, out cFiles))
            {
                await cFiles.Select();
            }
        }

        public static async Task Open(string targetID)
        {
            CFolder cFolder = null;
            CFile cFile = null;
            CFiles cFiles = null;
            if (CCache.FoldersCache.TryGetValue(targetID, out cFolder))
            {
                await cFolder.Open();
            }
            else if (CCache.FilesCache.TryGetValue(targetID, out cFile))
            {
                await cFile.Open();
            }
            else if (CCache.MFilesCache.TryGetValue(targetID, out cFiles))
            {
                await cFiles.Open();
            }
        }

        public static async Task<CFolder> CreateFolder(string folderID, string targetID, string folderName)
        {
            CFolder cFolder = CCache.GetFolderCache(targetID);
            CFolder newFolder = await cFolder.CreateFolder(folderID, folderName);
            return newFolder;
        }

        public static async Task<CFile> CreateFile(string fileID, string targetID, string fileName)
        {
            CFolder cFolder = CCache.GetFolderCache(targetID);
            CFile newFile = await cFolder.CreateFile(fileID, fileName);
            return newFile;
        }

        public static async Task Delete(string targetID)
        {
            CFolder cFolder = null;
            CFile cFile = null;
            CFiles cFiles = null;
            if (CCache.FoldersCache.TryGetValue(targetID, out cFolder))
            {
                await cFolder.Delete();
            }
            else if (CCache.FilesCache.TryGetValue(targetID, out cFile))
            {
                await cFile.Delete();
            }
            else if (CCache.MFilesCache.TryGetValue(targetID, out cFiles))
            {
                await cFiles.Delete();
            }
        }
    }
}
