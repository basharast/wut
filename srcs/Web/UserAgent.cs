using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
namespace WinUniversalTool.WebViewer
{
    public static class UserAgent
    {
        private const int URLMON_OPTION_USERAGENT = 0x10000001;
        private const int URLMON_OPTION_USERAGENT_REFRESH = 0x10000002;
        public static void SetDefaultUserAgent(string userAgent)
        {
            try
            {
                RuntimeLoader.PrepareUrlMkSettion();
                if (RuntimeLoader.urlMkSetSession != null && RuntimeLoader.istUrlMkSetSettionLoaded)
                {
                    RuntimeLoader.urlMkSetSession(URLMON_OPTION_USERAGENT_REFRESH, null, 0, 0);
                    if (userAgent.Length > 0)
                    {
                        var hr = RuntimeLoader.urlMkSetSession(URLMON_OPTION_USERAGENT, userAgent, userAgent.Length, 0);
                        var ex = Marshal.GetExceptionForHR(hr);
                        if (ex != null)
                        {
                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }
        public static string GetUserAgent()
        {
            try
            {
                RuntimeLoader.PrepareUrlMkSettion();
                int capacity = 255;
                var buf = new StringBuilder(capacity);
                int length = 0;

                RuntimeLoader.urlMkGetSession(URLMON_OPTION_USERAGENT, buf, capacity, ref length, 0);

                return buf.ToString();
            }
            catch (Exception e)
            {
                
            }
            return "Failed to get Agent details";
        }
    }

        public static class RuntimeLoader
        {
            //import necessary API as shown in other examples
            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

            [DllImport("kernel32", SetLastError = true)]
            public static extern void FreeLibrary(IntPtr module);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr module, string proc);

            public delegate int InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
            public static InternetSetOption InternetSet;

            public delegate int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);
            public static UrlMkSetSessionOption urlMkSetSession;

            public delegate int UrlMkGetSessionOption(int dwOption, StringBuilder pBuffer, int dwBufferLength, ref int pdwBufferLength, int dwReserved);
            public static UrlMkGetSessionOption urlMkGetSession;

            public static bool istUrlMkSetSettionLoaded = false;
            public static bool istUrlMkSetSettionFailed = false;
            public static IntPtr DLLModule = IntPtr.Zero;
            public static IntPtr DLLModule2 = IntPtr.Zero;
            public static string LoadDDLToTheMemory(string dllFile)
            {
                IntPtr intPtr = IntPtr.Zero;
                try
                {
                    intPtr = LoadLibrary(dllFile);
                }
                catch (Exception ex)
                {
                    return $"{ex.Message}\n{Marshal.GetLastWin32Error()}";
                }

                return intPtr.ToString();
            }

            public static string UnloadDllFromTheMemory(string pointer)
            {
                var output = $"Unload ({pointer}) done";
                try
                {
                    var pointerInt = long.Parse(pointer);
                    IntPtr intPtr = new IntPtr(pointerInt);
                    FreeLibrary(intPtr);
                }
                catch (Exception e)
                {
                    output = $"{e.Message}\n{Marshal.GetLastWin32Error()}";
                }

                return output;
            }

            public static void PrepareUrlMkSettion()
            {
                try
                {
                    if (istUrlMkSetSettionLoaded)
                    {
                        return;
                    }
                    try
                    {
                        DLLModule = LoadLibrary("urlmon");
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"DLLModule = LoadLibrary\n{e.Message}\n{Marshal.GetLastWin32Error()}");
                    }
                    if (DLLModule != IntPtr.Zero)
                    {
                        try
                        {
                            IntPtr UrlMkSetSessionOptionPointer = GetProcAddress(DLLModule, "UrlMkSetSessionOption");
                            if (UrlMkSetSessionOptionPointer != IntPtr.Zero) // error handling
                            {
                                urlMkSetSession = Marshal.GetDelegateForFunctionPointer<UrlMkSetSessionOption>(UrlMkSetSessionOptionPointer);
                                istUrlMkSetSettionLoaded = true;
                            }
                            else
                            {
                                istUrlMkSetSettionFailed = true;
                                throw new Exception($"Could not load method: {Marshal.GetLastWin32Error()}");
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"GetProcAddress(DLLModule, UrlMkSetSessionOption);\n{e.Message}\n{Marshal.GetLastWin32Error()}");
                        }
                        try
                        {
                            IntPtr UrlMkGetSessionOptionPointer = GetProcAddress(DLLModule, "UrlMkGetSessionOption");
                            if (UrlMkGetSessionOptionPointer != IntPtr.Zero) // error handling
                            {
                                urlMkGetSession = Marshal.GetDelegateForFunctionPointer<UrlMkGetSessionOption>(UrlMkGetSessionOptionPointer);
                                istUrlMkSetSettionLoaded = true;
                            }
                            else
                            {
                                istUrlMkSetSettionFailed = true;
                                throw new Exception($"Could not load method: {Marshal.GetLastWin32Error()}");
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"GetProcAddress(DLLModule, UrlMkGetSessionOption);\n{e.Message}\n{Marshal.GetLastWin32Error()}");
                        }
                    }
                    else
                    {
                        istUrlMkSetSettionFailed = true;
                        throw new Exception($"Could not load library: urlmon.dll");
                    }
                }
                catch (Exception e)
                {
                    istUrlMkSetSettionFailed = true;
                    throw new Exception($"{e.Message}");
                }
            }
        }
}
