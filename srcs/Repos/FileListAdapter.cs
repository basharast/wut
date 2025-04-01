using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.Models;

namespace WinUniversalTool.DirectStorage
{
    public static class FileListAdapter
    {
        public static ObservableCollectionEx<BrowseListModel> PackagesToFiles(List<PackageInfo> packageInfos)
        {
            ObservableCollectionEx<BrowseListModel> tempList = new ObservableCollectionEx<BrowseListModel>();

            try
            {
                uint rootID = LocalToData.crc32(Path.GetRandomFileName());

                foreach (var pItem in packageInfos)
                {
                    try
                    {
                        int id = (int)LocalToData.crc32(pItem.fileURL.ToString());
                        datas datas = new datas();
                        datas.id = id;
                        datas.parent_id = (int)rootID;
                        datas.file_name = pItem.fileName;
                        datas.name = pItem.fileName;
                        datas.file_size = pItem.fileSize;
                        datas.extension = Path.GetExtension(pItem.fileName);
                        datas.url = pItem.fileURL.ToString();
                        datas.path = pItem.fileURL.ToString();
                        datas.type = "file";
                        datas.hash = pItem.PackageItem.UpdateID;
                        datas.ignored = false;

                        Astifan2Node astifan2Node = new Astifan2Node(datas, "", "");
                        MegaFile megaFile = new MegaFile(astifan2Node, new System.Threading.CancellationToken());
                        megaFile.DirectRepo = true;
                        megaFile.DirectStore = true;
                        megaFile.DirectRepoLink = pItem.fileURL.ToString();
                        BrowseListModel browseListModel = new BrowseListModel(megaFile, new System.Threading.CancellationToken());
                        if (pItem.fileLogo?.ToString()?.Length > 0)
                        {
                            browseListModel.fileIcon = pItem.fileLogo.ToString();
                        }
                        tempList.Add(browseListModel);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                datas root = new datas();
                root.id = (int)rootID;
                root.file_name = "Store";
                root.name = "Store";
                root.type = "folder";
                root.parent_id = null;
                root.hash = Helpers.sha256_hash("store_root");
                root.ignored = false;
                Astifan2Node root2Node = new Astifan2Node(root, "", "");
                MegaRoot megaRoot = new MegaRoot(root2Node);
                BrowseListModel rootModel = new BrowseListModel(megaRoot);
                tempList.Insert(0, rootModel);
            }
            catch (Exception ex)
            {

            }

            return tempList;
        }
    }
}
