using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace EmbedInteropDll
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CreateFileInMemory(
                Application.StartupPath + @"\\AxInterop.WMPLib.dll",
                Assembly.GetExecutingAssembly().GetManifestResourceStream("EmbedInteropDll.AxInterop.WMPLib.dll"));

            CreateFileInMemory(
                Application.StartupPath + @"\\Interop.WMPLib.dll",
                Assembly.GetExecutingAssembly().GetManifestResourceStream("EmbedInteropDll.Interop.WMPLib.dll"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void CreateFileInMemory(string virtualPath, Stream stream)
        {
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];

            using (new SafeFileHandle(
                    BoxedAppSDK.NativeMethods.BoxedAppSDK_CreateVirtualFile(
                        virtualPath,
                        BoxedAppSDK.NativeMethods.EFileAccess.GenericWrite,
                        BoxedAppSDK.NativeMethods.EFileShare.Read,
                        IntPtr.Zero,
                        BoxedAppSDK.NativeMethods.ECreationDisposition.New,
                        BoxedAppSDK.NativeMethods.EFileAttributes.Normal,
                        IntPtr.Zero
                    ),
                    true
                )
            )
            {
            }

            int readBytes;

            using (FileStream virtualFileStream = new FileStream(virtualPath, FileMode.Open))
            {
                while ((readBytes = stream.Read(buffer, 0, bufferSize)) > 0)
                    virtualFileStream.Write(buffer, 0, readBytes);
            }
        }
    }
}
