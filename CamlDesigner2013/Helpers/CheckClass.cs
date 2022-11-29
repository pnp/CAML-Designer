using System;
using System.Runtime.InteropServices;

namespace CamlDesigner2013.Helpers
{
    // GAC Interfaces - IAssemblyCache. As a sample, non used vtable entries 
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
    internal interface IAssemblyCache
    {
        int Dummy1();
        [PreserveSig]
        IntPtr QueryAssemblyInfo(
            int flags,
            [MarshalAs(UnmanagedType.LPWStr)]
            string assemblyName,
            ref AssemblyInfo assemblyInfo);

        int Dummy2();
        int Dummy3();
        int Dummy4();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AssemblyInfo
    {
        public int checkbboxAssemblyInfo;
        public int assemblyFlags;
        public long assemblySizeInKB;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string currentAssemblyPath;

        public int cchBuf;
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CheckClass
    {
        public static bool AssemblyExist(string assemblyname)
        {
            try
            {
                string response = QueryAssemblyInfo(assemblyname);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // If assemblyName is not fully qualified, a random matching may be 
        private static string QueryAssemblyInfo(string assemblyName)
        {
            var assembyInfo = new AssemblyInfo { cchBuf = 512 };
            assembyInfo.currentAssemblyPath = new string('\0', assembyInfo.cchBuf);

            IAssemblyCache assemblyCache;

            // Get IAssemblyCache pointer
            var hr = GacApi.CreateAssemblyCache(out assemblyCache, 0);
            if (hr == IntPtr.Zero)
            {
                hr = assemblyCache.QueryAssemblyInfo(1, assemblyName, ref assembyInfo);
                if (hr != IntPtr.Zero)
                {
                    Marshal.ThrowExceptionForHR(hr.ToInt32());
                }
            }
            else
            {
                Marshal.ThrowExceptionForHR(hr.ToInt32());
            }
            return assembyInfo.currentAssemblyPath;
        }
    }

    internal class GacApi
    {
        [DllImport("fusion.dll")]
        internal static extern IntPtr CreateAssemblyCache(
            out IAssemblyCache pointerpAsmCache, int reserved);
    }
}
