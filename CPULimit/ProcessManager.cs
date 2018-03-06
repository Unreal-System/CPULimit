using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace CPULimit
{
    internal static class ProcessManager
    {
        //[Flags]
        //private enum ProcessAccessFlags : UInt32
        //{
        //    All = 0x001F0FFF,
        //    Terminate = 0x00000001,
        //    CreateThread = 0x00000002,
        //    VirtualMemoryOperation = 0x00000008,
        //    VirtualMemoryRead = 0x00000010,
        //    VirtualMemoryWrite = 0x00000020,
        //    DuplicateHandle = 0x00000040,
        //    CreateProcess = 0x000000080,
        //    SetQuota = 0x00000100,
        //    SetInformation = 0x00000200,
        //    QueryInformation = 0x00000400,
        //    QueryLimitedInformation = 0x00001000,
        //    Synchronize = 0x00100000
        //}

        [Flags]
        private enum ProcessAccess : UInt32
        {
            Terminate = 0x1,
            CreateThread = 0x2,
            SetSessionId = 0x4,
            VmOperation = 0x8,
            VmRead = 0x10,
            VmWrite = 0x20,
            DupHandle = 0x40,
            CreateProcess = 0x80,
            SetQuota = 0x100,
            SetInformation = 0x200,
            QueryInformation = 0x400,
            SetPort = 0x800,
            SuspendResume = 0x800,
            QueryLimitedInformation = 0x1000,
            Synchronize = 0x100000
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern IntPtr NtResumeProcess(IntPtr ProcessHandle);

        [DllImport("ntdll.dll", SetLastError = false)]
        private static extern IntPtr NtSuspendProcess(IntPtr ProcessHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(ProcessAccess processAccess, Boolean bInheritHandle, Int32 processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        /*internal static void ControlProcess(Process proc)
        {
            IntPtr hProc = IntPtr.Zero;
            try
            {
                hProc = OpenProcess(ProcessAccessFlags.CreateProcess, false, proc.Id);
                if (hProc != IntPtr.Zero)
                    NtSuspendProcess(hProc);
                if (hProc != IntPtr.Zero)
                    NtResumeProcess(hProc);
            }
            finally
            {
                CloseHandle(hProc);
            }
        }*/

        /// <summary>
        /// Pause Process
        /// </summary>
        /// <param name="processId"></param>
        internal static void SuspendProc(Int32 processId)
        {
            IntPtr hProc = IntPtr.Zero;
            try
            {
                hProc = OpenProcess(ProcessAccess.SuspendResume, false, processId);
                if (hProc != IntPtr.Zero)
                    NtSuspendProcess(hProc);
            }
            finally
            {
                CloseHandle(hProc);
            }
        }

        /// <summary>
        /// Resume Process
        /// </summary>
        /// <param name="processId"></param>
        internal static void ResumeProc(Int32 processId)
        {
            IntPtr hProc = IntPtr.Zero;
            try
            {
                hProc = OpenProcess(ProcessAccess.SuspendResume, false, processId);
                if (hProc != IntPtr.Zero)
                    NtResumeProcess(hProc);
            }
            finally
            {
                CloseHandle(hProc);
            }
        }
    }
}