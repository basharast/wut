/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinUniversalTool.MegaApiClientSource;
using WinUniversalTool.Models;

namespace WinUniversalTool.DirectStorage
{
    public class Astifan2Node : INode
    {
        public Astifan2Node(datas Datas, string APIToken, string hostURL, string rootID = "", bool isLocalStorage = false)
        {
            if (Datas != null)
            {
                try
                {
                    ignored = Datas.ignored;
                    parentId = Datas.parent_id?.ToString();
                    creationDate = new DateTime();
                    authKey = APIToken;
                    this.hostURL = hostURL;
                    this.rootID = rootID;
                    if (isLocalStorage)
                    {
                        LocalToken = APIToken;
                    }
                    try
                    {
                        DateTime.TryParse(Datas.created_at, out creationDate);
                    }
                    catch (Exception e)
                    {

                    }
                    try
                    {
                        if (Datas.users != null)
                        {
                            owner = Datas.users[0].display_name;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    id = Datas.id.ToString();


                    type = NodeType.Directory;
                    var itemType = Datas.type;
                    switch (itemType)
                    {
                        case "folder":
                            if (Datas.parent_id == null)
                            {
                                type = NodeType.Root;
                            }
                            else
                            {
                                type = NodeType.Directory;
                            }
                            break;

                        default:
                            type = NodeType.File;
                            break;
                    }


                    name = Datas.name?.ToString();

                    if (Datas.file_size != null)
                    {
                        size = (long)Datas.file_size;
                    }
                    else
                    {
                        size = 0;
                    }

                    modificationDate = new DateTime();
                    try
                    {
                        modificationDate = DateTime.Parse(Datas.updated_at);
                    }
                    catch (Exception e)
                    {

                    }

                    serializedFingerprint = Datas.hash?.ToString();
                }
                catch (Exception e)
                {
                    Helpers.Logger(e);
                }
            }
        }

        bool ignored;
        public bool Ignored
        {
            get
            {
                return ignored;
            }
        }
        string parentId;
        public string ParentId
        {
            get
            {
                return parentId;
            }
            set
            {
                parentId = value;
            }
        }

        string localToken;
        public string LocalToken
        {
            get
            {
                return localToken;
            }
            set
            {
                localToken = value;
            }
        }

        string authKey;
        public string AuthKey
        {
            get
            {
                return authKey;
            }
            set
            {
                authKey = value;
            }
        }
        string hostURL;
        public string HostURL
        {
            get
            {
                return hostURL;
            }
            set
            {
                hostURL = value;
            }
        }

        DateTime creationDate;
        public DateTime CreationDate
        {
            get
            {

                return creationDate;
            }
        }

        string owner;
        public string Owner
        {
            get
            {
                return owner;
            }
        }

        string id;
        public string Id
        {
            get
            {
                return id;
            }
        }

        NodeType type;
        public NodeType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        string RootID;
        public string rootID
        {
            get
            {
                return RootID;
            }
            set
            {
                RootID = value;
            }
        }

        long size;
        public long Size
        {
            get
            {
                return size;
            }
        }

        DateTime? modificationDate;
        public DateTime? ModificationDate
        {
            get
            {
                return modificationDate;
            }
        }

        public string serializedFingerprint;
        public string SerializedFingerprint
        {
            get
            {
                return serializedFingerprint;
            }
        }

        public bool AstifanStorage
        {
            get
            {
                return true;
            }
        }

        public IFileAttribute[] FileAttributes
        {
            get
            {
                return null;
            }
        }

        public bool Equals(INodeInfo other)
        {
            return this.Equals(other);
        }
    }
}
