using System;
using System.Runtime.InteropServices;

namespace DiskSpaceAnalyse
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public class SHFILEOPSTRUCTWWIN32
    {
        public IntPtr Hwnd { get; set; }
        public uint Func { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public ushort Flags { get; set; }
        public int AnyOperationsAborted { get; set; }
        public IntPtr NameMappings { get; set; }
        public string ProgressTitle { get; set; }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class SHFILEOPSTRUCTWWIN64
    {
        public IntPtr Hwnd { get; set; }
        public uint Func { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public ushort Flags { get; set; }
        public int AnyOperationsAborted { get; set; }
        public IntPtr NameMappings { get; set; }
        public string ProgressTitle { get; set; }
    }
}