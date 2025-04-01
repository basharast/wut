/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Commander.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Commander.Compiler
{
    public static class CCommands
    {
        //All Commands
        public static List<CCommand> ALLCOMMANDS = new List<CCommand>();

        //Script Header
        public readonly static CCommand SCRIPTNAME = new CCommand("script-name", new string[] { "name"}, "Define script name", true);
        public readonly static CCommand SCRIPTAUTHOR = new CCommand("script-author", new string[] { "name"}, "Define author name", true);
        public readonly static CCommand SCRIPTVERSION = new CCommand("script-version", new string[] { "version"}, "Define script version", true);
        public readonly static CCommand SCRIPTYEAR = new CCommand("script-year", new string[] { "year"}, "Define script release year", true);

        //Script Variables
        public readonly static CCommand DEFINE = new CCommand("define", new string[] { "variable", "value" }, "Define variable with custom value", true);
        public readonly static CCommand DEFINEC = new CCommand("definec", new string[] { "variable", "command" }, "Define variable with custom value", true);
        public readonly static CCommand FUNC = new CCommand("func", new string[] { "variable", "command" }, "Define variable with custom value", true);
        public readonly static CCommand GET = new CCommand("getx", new string[] { "variable", "[property]" }, "Get variable property, used with objects", true);
        public readonly static CCommand SET = new CCommand("set", new string[] { "variable", "[property]", "value" }, "set variable property, used with objects", true);

        //Script Areas
        public readonly static CCommand AREA = new CCommand("area", new string[] { "areaID", "command" }, "Define new area with custom command", true);
        public readonly static CCommand JUMP = new CCommand("jump", new string[] { "areaID" }, "Jump to specific area", true);
        public readonly static CCommand APPRECORD = new CCommand("apprecord", new string[] { "folderID", "recordName", "targetLayout" }, "Jump to specific area", true);

        //Script Runtime
        public readonly static CCommand DELAY = new CCommand("delay", new string[] { "time" }, "Delay the the below code for 'x' seconds, '0.x' for less", true);
        public readonly static CCommand BREAK = new CCommand("break", new string[] { "message" }, "Break the entire script and push notification message", true);
        public readonly static CCommand BREAKWHEN = new CCommand("breakwhen", new string[] {"condition" ,"message" }, "Break the entire script and push notification message", true);
        public readonly static CCommand STOP = new CCommand("stop", new string[] { "targetID" }, "Stop specific task/timer by ID", true);
        public readonly static CCommand START = new CCommand("start", new string[] { "targetID" }, "Start specific task/timer by ID", true);
        public readonly static CCommand CLOSE = new CCommand("close", new string[] { "telnetID" }, "Close specific telnet by ID", true);

        //Script Events
        public readonly static CCommand ONSTART = new CCommand("onStart", new string[] { "targetID", "processID" }, "Call process on task/timer/process start", true);
        public readonly static CCommand ONFINISH = new CCommand("onFinish", new string[] { "targetID", "processID" }, "Call process on task/timer/process finish", true);
        public readonly static CCommand ONCLOSE = new CCommand("onClose", new string[] { "targetID", "processID" }, "Call process on task/timer/process close", true);
        public readonly static CCommand ONCLICK = new CCommand("onClick", new string[] { "targetID", "processID" }, "Call process on button click", true);
         
        
        public readonly static CCommand REPLACE = new CCommand("replace", new string[] { "targetID", "target", "new" }, "Call process on button click", true);
        public readonly static CCommand REPLACED = new CCommand("replaced", new string[] { "variableID","targetID", "target", "new" }, "Call process on button click", true);
        public readonly static CCommand REPLACER = new CCommand("replacer", new string[] { "variableID","targetID", "target", "new" }, "Call process on button click", true);
        public readonly static CCommand SUBSTR = new CCommand("substr", new string[] { "variableID","targetID", "start", "end" }, "Call process on button click", true);
        public readonly static CCommand SUBARR = new CCommand("subarr", new string[] { "variableID","targetID", "separator", "start", "end" }, "Call process on button click", true);
        public readonly static CCommand STRLEN = new CCommand("length", new string[] { "variableID","targetID" }, "Call process on button click", true);
        public readonly static CCommand ARRYCOUNT = new CCommand("count", new string[] { "variableID","targetID", "separator" }, "Call process on button click", true);
        public readonly static CCommand PLAY = new CCommand("play", new string[] { "folderID", "fileName", "volume" }, "Call process on button click", true);
        public readonly static CCommand CLIPBOARD = new CCommand("clipboard", new string[] { "title", "message", "text" }, "Call process on button click", true);
        public readonly static CCommand REFORMAT = new CCommand("reformat", new string[] { "variableID", "values1", "values2", "mixSymbol", "separator" }, "Call process on button click", true);
        public readonly static CCommand UNIQUE = new CCommand("unique", new string[] { "variableID", "arrayID", "separator" }, "Call process on button click", true);
        public readonly static CCommand SORT = new CCommand("sort", new string[] { "variableID", "arrayID", "separator", "sortType" }, "Call process on button click", true);

        //Script Tasks
        public readonly static CCommand LOAD = new CCommand("load", new string[] { "variableID", "dllFile" }, "Create new task with custom process", true);
        public readonly static CCommand UNLOAD = new CCommand("unload", new string[] { "variableID", "outputID" }, "Create new task with custom process", true);
        public readonly static CCommand TASK = new CCommand("task", new string[] { "taskID", "processID" }, "Create new task with custom process", true);
        public readonly static CCommand TIMER = new CCommand("timer", new string[] { "timerID", "processID", "interval" }, "Create new timer with custom process", true);
        public readonly static CCommand PROCESS = new CCommand("process", new string[] { "processID", "command", "telnetID" }, "Create new process with cmd command or .bat file, you can include parameters", true);

        //Script Telnet
        public readonly static CCommand TELNET = new CCommand("telnet", new string[] { "telnetID", "hostIP", "hostPort" }, "Create new telnet connection", true);
        public readonly static CCommand TELNETP = new CCommand("telnetp", new string[] { "telnetID", "hostIP", "hostPort", "tpass", "tuser" }, "Create new telnet connection", true);
        public readonly static CCommand SEND = new CCommand("send", new string[] { "telnetID", "command" }, "Send direct command using telnet", true);
        public readonly static CCommand RECONNECT = new CCommand("reconnect", new string[] { "telnetID" }, "Reconnect specific telnet client", true);

        //Script Conditions
        public readonly static CCommand WHEN = new CCommand("when", new string[] { "statement", "command" }, "Direct the script to another route when the statement is true", true);
        public readonly static CCommand IF = new CCommand("is", new string[] { "statement", "command" }, "Direct the script to another route when the statement is true", true);
        public readonly static CCommand ELSE = new CCommand("else", new string[] { "statement", "command" }, "Direct the script to another route when the statement is false", true);


        //Native Commands (Storage)
        public readonly static CCommand PICKFOLDER = new CCommand("folder", new string[] { "folderID", "extentions" }, "Invoke open folder dialog, will return storage folder object", true);
        public readonly static CCommand PICKFOLDERF = new CCommand("folderf", new string[] { "folderID", "parentID", "folderName" }, "Invoke open folder dialog, will return storage folder object", true);
        public readonly static CCommand PICKFILE = new CCommand("file", new string[] { "fileID", "extentions" }, "Invoke open file dialog, will return storage file object", true);
        public readonly static CCommand PICKFILEF = new CCommand("filef", new string[] { "fileID","folderID", "fileName" }, "Invoke open file dialog, will return storage file object", true);
        public readonly static CCommand PICKFILEL = new CCommand("filel", new string[] { "fileID", "fileLocation" }, "Invoke open file dialog, will return storage file object", true);
        public readonly static CCommand PICKFILES = new CCommand("files", new string[] { "filesID", "extentions" }, "Invoke open files dialog, will return storage files list", true);
        public readonly static CCommand PICKFILESF = new CCommand("filesf", new string[] { "filesID", "folderID", "extentions" }, "Invoke open files dialog, will return storage files list", true);
        public readonly static CCommand PICK = new CCommand("pick", new string[] { "targetID" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand GETFILE = new CCommand("get", new string[] { "targetID", "variableID" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand ENCRYPTFILE = new CCommand("encrypt", new string[] { "fileID", "folderID" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand DECRYPTFILE = new CCommand("decrypt", new string[] { "fileID", "folderID" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand REGEX = new CCommand("regex", new string[] { "variableID", "regexPattern", "targetID" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand REGEXG = new CCommand("regexg", new string[] { "variableID", "regexPattern", "group", "targetID" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand REGEXC = new CCommand("regexc", new string[] { "variableID", "regexPattern", "group", "targetID" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand APPEND = new CCommand("append", new string[] { "targetID", "content" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand WRITE = new CCommand("write", new string[] { "targetID", "content" }, "reselect file/folder with specific ID", true);
        
        public readonly static CCommand FORM = new CCommand("form", new string[] { "formID", "title", "containerID", "button1", "button2" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand ELEMENT = new CCommand("element", new string[] { "elementID", "type", "value", "description", "hint", "tabName" }, "reselect file/folder with specific ID", true);
        public readonly static CCommand CONTAINER = new CCommand("container", new string[] { "containerID", "tabsNames" }, "reselect file/folder with specific ID", true);
        
        public readonly static CCommand WAITF = new CCommand("waitf", new string[] { "filename", "message", "timeout", "command" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITFD = new CCommand("waitfd", new string[] { "folderID", "filename", "message", "timeout", "command" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITN = new CCommand("waitn", new string[] { "title", "message", "delay" }, "Open file/folder with default app", true);
       
        public readonly static CCommand WAITB = new CCommand("waitb", new string[] { "title" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITIC = new CCommand("waitic", new string[] { "message" }, "Open file/folder with default app", true);
        public readonly static CCommand CANCELB = new CCommand("cancelb", new string[] { "title" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITD = new CCommand("waitd", new string[] { "filename","title" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITE = new CCommand("waite", new string[] { "filename","title" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITI = new CCommand("waiti", new string[] { "filename", "outputVar","title" }, "Open file/folder with default app", true);
        public readonly static CCommand DOWNLOADR = new CCommand("queuer", new string[] { "filename" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITP = new CCommand("waitp", new string[] { "message", "target" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITPR = new CCommand("waitpr", new string[] { "message", "processName" }, "Open file/folder with default app", true);
        public readonly static CCommand WAITHB = new CCommand("waithb", new string[] { "title" }, "Open file/folder with default app", true);
        public readonly static CCommand CANCELHB = new CCommand("cancelhb", new string[] { "title" }, "Open file/folder with default app", true);
        public readonly static CCommand DOWNLOAD = new CCommand("download", new string[] { "downloadID", "filelink" }, "Open file/folder with default app", true);
        public readonly static CCommand DOWNLOADF = new CCommand("downloadf", new string[] { "downloadID", "folderID", "fileName" }, "Open file/folder with default app", true);
        public readonly static CCommand DOWNLOADFD = new CCommand("downloadfd", new string[] { "downloadID", "folderID", "fileName", "saveFolderID" }, "Open file/folder with default app", true);
        public readonly static CCommand INCLUDE = new CCommand("include", new string[] { "fileID" }, "Open file/folder with default app", true);
        public readonly static CCommand INCLUDEL = new CCommand("includel", new string[] { "fileLocation" }, "Open file/folder with default app", true);
        public readonly static CCommand LOOP = new CCommand("loop", new string[] { "start", "end", "step", "command" }, "Open file/folder with default app", true);
        public readonly static CCommand EACH = new CCommand("each", new string[] { "arrayID", "separator", "command" }, "Open file/folder with default app", true);
        public readonly static CCommand BROWSE = new CCommand("browse", new string[] { "weblink" }, "Open file/folder with default app", true);
        public readonly static CCommand MINTARGET = new CCommand("mintarget", new string[] { "appTarget" }, "Open file/folder with default app", true);
        public readonly static CCommand LURI = new CCommand("luri", new string[] { "weblink" }, "Open file/folder with default app", true);
        public readonly static CCommand LURIF = new CCommand("lurif", new string[] { "weblink","fileID" }, "Open file/folder with default app", true);
        public readonly static CCommand BROWSER = new CCommand("browser", new string[] { "weblink" }, "Open file/folder with default app", true);
        public readonly static CCommand OPENFILE = new CCommand("open", new string[] { "targetID" }, "Open file/folder with default app", true);
        public readonly static CCommand OPENFILEL = new CCommand("openl", new string[] { "fileName" }, "Open file/folder with default app", true);
        public readonly static CCommand GETL = new CCommand("getl", new string[] { "fileName", "variableID" }, "Open file/folder with default app", true);
        public readonly static CCommand OPENFILEF = new CCommand("openf", new string[] { "folderID","fileName" }, "Open file/folder with default app", true);
        public readonly static CCommand QUEUEF = new CCommand("queuef", new string[] { "queueID", "folderID","fileName" }, "Open file/folder with default app", true);
        public readonly static CCommand QUEUEL = new CCommand("queuel", new string[] { "queueID","fileName" }, "Open file/folder with default app", true);
        public readonly static CCommand REGISTER = new CCommand("register", new string[] { "resultID","fileName","devstate" }, "Open file/folder with default app", true);
        public readonly static CCommand CREATEFOLDER = new CCommand("create", new string[] { "folderID", "parentID", "foldername" }, "Create new folder in specific folder", true);
        public readonly static CCommand CREATEFOLDERA = new CCommand("createa", new string[] { "folderID", "parentID", "foldername" }, "Create new folder in specific folder", true);
        public readonly static CCommand CREATEFILE = new CCommand("createf", new string[] { "fileID", "folderID", "filename" }, "Create new file in folder", true);
        public readonly static CCommand CREATEFILEA = new CCommand("createfa", new string[] { "fileID", "folderID", "filename" }, "Create new file in folder", true);
        public readonly static CCommand DELETEFILE = new CCommand("delete", new string[] { "targetID" }, "Delete file/folder", true);
        public readonly static CCommand DELETEFILEF = new CCommand("deletef", new string[] { "targetID", "fileName" }, "Delete file/folder", true);
        public readonly static CCommand CLEANALL = new CCommand("clean", new string[] { "all" }, "Delete file/folder", true);
        public readonly static CCommand CLEARALL = new CCommand("clear", new string[] { "all" }, "Delete file/folder", true);
        public readonly static CCommand OCR = new CCommand("ocr", new string[] { "outputID", "fileID", "lang" }, "Delete file/folder", true);


        //Native Commands (Dialogs)
        public readonly static CCommand LISTDIALOG = new CCommand("list"   , new string[] { "dialogID", "title", "data","header", "button1", "button2" }, "Show input dialog, will return string value", true);
        public readonly static CCommand LISTMDIALOG = new CCommand("listm"   , new string[] { "dialogID", "title", "data", "header", "button1", "button2" }, "Show input dialog, will return string value", true);
        public readonly static CCommand INPUTDIALOG = new CCommand("input"   , new string[] { "dialogID", "title", "placeholder", "button1", "button2" }, "Show input dialog, will return string value", true);
        public readonly static CCommand IMAGEDIALOG = new CCommand("image"   , new string[] { "dialogID", "title", "fileID", "button1", "button2" }, "Show input dialog, will return string value", true);
        public readonly static CCommand VIDEODIALOG = new CCommand("video"   , new string[] { "dialogID", "title", "fileID", "button1", "button2" }, "Show input dialog, will return string value", true);
        public readonly static CCommand SLIDERDIALOG = new CCommand("slider"   , new string[] { "dialogID", "title", "range", "button1", "button2" }, "Show input dialog, will return string value", true);
        public readonly static CCommand COMBODIALOG = new CCommand("combo", new string[] { "dialogID", "title", "list", "placeholder", "button1", "button2" }, "Show input dialog, will return string value", true);
        public readonly static CCommand COMBODIALOGI = new CCommand("comboi", new string[] { "dialogID", "title", "list", "placeholder", "extraElement", "button1", "button2" }, "Show input dialog, will return string value", true);
        public readonly static CCommand ASKDIALOG = new CCommand("ask", new string[] { "dialogID", "title", "question", "button1", "button2" }, "Show question dialog, will return button value", true);
        public readonly static CCommand INFODIALOG = new CCommand("info", new string[] { "info", "message", "button"}, "Show info dialog, will return button value or null on backpressed", true);
        public readonly static CCommand TEXTDIALOG = new CCommand("text", new string[] { "title", "content", "button"}, "Show info dialog, will return button value or null on backpressed", true);
        public readonly static CCommand REPO = new CCommand("repo", new string[] { "folderID","hostName", "RepoID", "RepoKey", "fileName" }, "Show info dialog, will return button value or null on backpressed", true);
        public readonly static CCommand UPDATE = new CCommand("update", new string[] { "updateLink" }, "Show info dialog, will return button value or null on backpressed", true);
        public readonly static CCommand EXISTS = new CCommand("exists", new string[] { "variableID","fileFullLocation" }, "Show info dialog, will return button value or null on backpressed", true);
        public readonly static CCommand CRC = new CCommand("crc", new string[] { "variableID","fileID" }, "Show info dialog, will return button value or null on backpressed", true);
        public readonly static CCommand CRCL = new CCommand("crcl", new string[] { "variableID","filePath" }, "Show info dialog, will return button value or null on backpressed", true);

        //Native Commands (Notifications)
        public readonly static CCommand NOTIFICATION = new CCommand("notify", new string[] { "title", "message", "timeout" }, "Push notification with custom timeout, delay true-false will hold the script until notification pressed", true);
        public readonly static CCommand NOTIFICATIONL = new CCommand("notifyl", new string[] { "type", "message","icon", "timeout" }, "Push notification with custom timeout, delay true-false will hold the script until notification pressed", true);

        //Native Commands (Output)
        public readonly static CCommand ICON = new CCommand("icon", new string[] { "path" }, "Change the current icon", true);
        public readonly static CCommand STATUS = new CCommand("status", new string[] { "message" }, "Show status message for the user", true);
        public readonly static CCommand STATUST = new CCommand("statust", new string[] { "message" }, "Show status message for the user", true);
        public readonly static CCommand STATUSD = new CCommand("statusd", new string[] { "message" }, "Show status message for the user", true);
        public readonly static CCommand LOG = new CCommand("log", new string[] { "message" }, "Add log message to log file", true);
        public readonly static CCommand CMD = new CCommand("cmd", new string[] { "cmdcomm" }, "Add log message to log file", true);
        public readonly static CCommand CMDOUT = new CCommand("cmdout", new string[] { "outputID", "command" }, "Add log message to log file", true);
        public readonly static CCommand WHENC = new CCommand("whenc", new string[] { "cmdcomm" }, "Add log message to log file", true);
        public readonly static CCommand CALL = new CCommand("call", new string[] { "cmdcomm" }, "Add log message to log file", true);
        public readonly static CCommand RAND = new CCommand("rand", new string[] { "variableID", "min", "max" }, "Add log message to log file", true);
        public readonly static CCommand BARP = new CCommand("barp", new string[] { "state" }, "Add log message to log file", true);
        public readonly static CCommand SPEAK = new CCommand("speak", new string[] { "text", "gender" }, "Add log message to log file", true);
        public readonly static CCommand SPEAKF = new CCommand("speakf", new string[] { "text", "gender", "fileID" }, "Add log message to log file", true);
        public readonly static CCommand SPEAKR = new CCommand("speakr", new string[] { "variableID", "type"}, "Add log message to log file", true);
        public readonly static CCommand DEBUG = new CCommand("debug", new string[] { "state" }, "Add log message to log file", true);
        public readonly static CCommand RCASE = new CCommand("rcase", new string[] { "state" }, "Add log message to log file", true);
        public readonly static CCommand HOLD = new CCommand("hold", new string[] { "state" }, "Add log message to log file", true);
        public readonly static CCommand RESPONSE = new CCommand("response", new string[] { "responseID","url" }, "Add log message to log file", true);
        public readonly static CCommand BACKW = new CCommand("backw", new string[] { "state" }, "Add log message to log file", true);
        public readonly static CCommand PSAVE = new CCommand("psave", new string[] { "variableID", "value" }, "Add log message to log file", true);
        public readonly static CCommand PGET = new CCommand("pget", new string[] { "variableID", "default" }, "Add log message to log file", true);
        public readonly static CCommand PREMOVE = new CCommand("premove", new string[] { "variableID" }, "Add log message to log file", true);
        public readonly static CCommand GPIOOPEN = new CCommand("gopen", new string[] { "variableID", "pinNumber" }, "Add log message to log file", true);
        public readonly static CCommand GPIOWRITE = new CCommand("gwrite", new string[] { "variableID", "state" }, "Add log message to log file", true);
        public readonly static CCommand GREAD = new CCommand("gread", new string[] { "variableID", "targetID" }, "Add log message to log file", true);
        public readonly static CCommand GCLOSE = new CCommand("gclose", new string[] { "variableID" }, "Add log message to log file", true);

        //Native Commands (Elements)
        public readonly static CCommand BUTTON = new CCommand("button", new string[] { "buttonID", "text", "width" }, "create new button, you can use onClick", true);
        public readonly static CCommand PROGRESS = new CCommand("progress", new string[] { "value" }, "create new button, you can use onClick", true);
        public readonly static CCommand PROGRESSLINK = new CCommand("progresslink", new string[] { "folderID", "fileName" }, "create new button, you can use onClick", true);
        public readonly static CCommand STATUSLINK = new CCommand("statuslink", new string[] { "folderID", "fileName" }, "create new button, you can use onClick", true);

        //Native Commands (Advanced)
        //framerate|bitrate|width|height|fps|audio
        public readonly static CCommand IMAGES2VIDEO = new CCommand("images2video", new string[] { "folderID", "outputFile", "imagesFolder", "imagesType", "options" }, "Convert images to video", true);
        public readonly static CCommand UNZIP = new CCommand("unzip", new string[] { "fileID", "folderID" }, "Unzip file to specific folder", true);
        public readonly static CCommand ZIP = new CCommand("zip", new string[] { "sourceID", "targetID", "filename" }, "Unzip file to specific folder", true);

    }
}
