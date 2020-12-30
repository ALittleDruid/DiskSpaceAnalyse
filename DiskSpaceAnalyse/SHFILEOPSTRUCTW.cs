using System;
using System.Runtime.InteropServices;

namespace DiskSpaceAnalyse
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct SHFILEOPSTRUCTWWIN32
    {
        public IntPtr Hwnd;
        public uint Func;
        public string From;
        public string To;
        public ushort Flags;
        public int AnyOperationsAborted;
        public IntPtr NameMappings;
        public string ProgressTitle;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHFILEOPSTRUCTWWIN64
    {
        public IntPtr Hwnd;
        public uint Func;
        public string From;
        public string To;
        public ushort Flags;
        public int AnyOperationsAborted;
        public IntPtr NameMappings;
        public string ProgressTitle;
    }
}