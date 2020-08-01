using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace SecondService
{
    class ServiceUtils
    {
        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        private struct SECURITY_ATTRIBUTES
        {
            public int Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STARTUPINFO
        {
            public int cb;
            public String lpReserved;
            public String lpDesktop;
            public String lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        #endregion

        #region Enumerations

        enum TOKEN_TYPE : int
        {
            TokenPrimary = 1,
            TokenImpersonation = 2
        }

        enum SECURITY_IMPERSONATION_LEVEL : int
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3,
        }

        #endregion

        #region Constants

        private const int TOKEN_DUPLICATE = 0x0002;
        private const uint MAXIMUM_ALLOWED = 0x2000000;
        private const int CREATE_NEW_CONSOLE = 0x00000010;
        private const int HIGH_PRIORITY_PROCESS = 0x80;

        #endregion

        #region Win32 API Imports

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
            String lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll")]
        private static extern bool ProcessIdToSessionId(uint dwProcessId, ref uint pSessionId);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
        private extern static bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType,
            int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurityAttribute]
        private static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        #endregion


        private static List<int> GetSessionsWithLogger(String loggerFile) // int or uint?
        {
            List<int> ret = new List<int>();
            Process[] processes = Process.GetProcessesByName(loggerFile);
            foreach (Process p in processes)
                ret.Add(p.SessionId);

            return ret;
        }

        private static void RunLoggerForUser(uint winlogonPid, String loggerPath)
        {
            int creationFlags = HIGH_PRIORITY_PROCESS | CREATE_NEW_CONSOLE;

            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            IntPtr process = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);

            if (!OpenProcessToken(process, TOKEN_DUPLICATE, ref token))
            {
                CloseHandle(process);
            }

            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);

            if (!DuplicateTokenEx(token, MAXIMUM_ALLOWED, ref sa, (int)SECURITY_IMPERSONATION_LEVEL.SecurityDelegation, (int)TOKEN_TYPE.TokenPrimary, ref tokenDuplicate))
            {
                CloseHandle(process);
                CloseHandle(token);
            }

            STARTUPINFO si = new STARTUPINFO();
            si.cb = (int)Marshal.SizeOf(si);


            PROCESS_INFORMATION info = new PROCESS_INFORMATION();

            CreateProcessAsUser(tokenDuplicate,        // client's access token
                                loggerPath,            // file to execute
                                null,
                                ref sa,                 // pointer to process SECURITY_ATTRIBUTES
                                ref sa,                 // pointer to thread SECURITY_ATTRIBUTES
                                false,                  // handles are not inheritable
                                creationFlags,          // creation flags
                                IntPtr.Zero,            // pointer to new environment block 
                                null,                   // name of current directory 
                                ref si,                 // pointer to STARTUPINFO structure
                                out info
                                );

            // invalidate the handles
            CloseHandle(process);
            CloseHandle(token);
            CloseHandle(tokenDuplicate);
        }

        public static void startLoggerForAllUsers(String loggerPath)
        {
            String loggerName = Path.GetFileNameWithoutExtension(loggerPath);

            List<int> sessionsWithLogger = GetSessionsWithLogger(loggerName);
            Process[] processes = Process.GetProcessesByName("explorer");
            foreach (Process p in processes)
                if (!sessionsWithLogger.Contains(p.SessionId))
                    RunLoggerForUser((uint)p.Id, loggerPath);
        }
    }
}
