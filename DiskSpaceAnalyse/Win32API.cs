using System;
using System.Runtime.InteropServices;

namespace DiskSpaceAnalyse
{
    public class Win32API
    {
        [DllImport("Shell32")]
        public static extern int SHFileOperationW(IntPtr handle);

    }
}
