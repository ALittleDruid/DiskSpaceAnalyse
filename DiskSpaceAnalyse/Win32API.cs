using System;
using System.Runtime.InteropServices;

namespace DiskSpaceAnalyse
{
    public class Win32API
    {
        [DllImport("Shell32", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation([In]ref SHFILEOPSTRUCTWWIN32 handle);

        [DllImport("Shell32", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation([In] ref SHFILEOPSTRUCTWWIN64 handle);

    }
}
