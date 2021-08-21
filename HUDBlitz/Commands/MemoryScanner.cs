using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HUDBlitz.Commands
{
    public class MemoryScanner
    {
        public delegate void MemoryEditorStateHandler();
        MemoryEditorStateHandler _del;

        public void RegisterHandler(MemoryEditorStateHandler del)
        {
            _del += del;
        }

        public void UnregisterHandler(MemoryEditorStateHandler del)
        {
            _del -= del;
        }

        public MemoryScanner(string process_name)
        {
            this.pName = process_name;
        }

        public int GetBaseAddress()
        {
            Process process = Process.GetProcessesByName("wotblitz")[0];
            BaseAddress = (int)process.MainModule.BaseAddress;
            return this.BaseAddress;
        }

        public int GetProcessByName()
        {
            this.pID = 0;
            IntPtr handleToSnapshot = IntPtr.Zero;
            try
            {
                PROCESSENTRY32 procEntry = new PROCESSENTRY32();
                procEntry.dwSize = (UInt32)Marshal.SizeOf(typeof(PROCESSENTRY32));
                handleToSnapshot = CreateToolhelp32Snapshot((uint)SnapshotFlags.Process, 0);
                if (Process32First(handleToSnapshot, ref procEntry))
                {
                    do
                    {
                        if (this.pName == procEntry.szExeFile)
                        {
                            this.pID = (int)procEntry.th32ProcessID;
                            break;
                        }
                    } while (Process32Next(handleToSnapshot, ref procEntry));
                }
                else
                {
                    throw new ApplicationException(string.Format("Failed with win32 error code {0}", Marshal.GetLastWin32Error()));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Can't get the process.", ex);
            }
            finally
            {
                CloseHandle(handleToSnapshot);
            }
            return this.pID;
        }

        public byte[] ReadMemory(IntPtr address, uint size)
        {
            byte[] buffer = new byte[size];
            IntPtr bytesRead = IntPtr.Zero;
            IntPtr hProcess = OpenProcess(ProcessAccessFlags.VirtualMemoryRead, false, this.pID);
            ReadProcessMemory(hProcess, address, buffer, size, out bytesRead);
            CloseHandle(hProcess);
            return buffer;
        }

        public int ReadPointer(int addr, Int32[] offsets)
        {
            for (int i = 0; i < offsets.Length; i++)
            {
                addr = BitConverter.ToInt32(ReadMemory((IntPtr)addr, (uint)4), 0) + offsets[i];
            }
            return addr;
        }

        public void GetDamage(object sender, EventArgs e)
        {
            Int32 DealtAddress = BaseAddress + 0x034084A0; 
            Int32[] DealtOffset = { 0x3C, 0x98, 0x70, 0x34, 0x1C };
            Dealt = BitConverter.ToInt32(ReadMemory((IntPtr)ReadPointer(DealtAddress, DealtOffset), (uint)4), 0);

            Int32 ReceivedAddress = BaseAddress + 0x034084A0;
            Int32[] ReceivedOffset = { 0x3C, 0x98, 0x70, 0x38, 0x1C };
            Received = BitConverter.ToInt32(ReadMemory((IntPtr)ReadPointer(ReceivedAddress, ReceivedOffset), (uint)4), 0);

            Int32 StrengthAddress = BaseAddress + 0x034084A0;
            Int32[] StrengthOffset = { 0x9C, 0x0, 0x10, 0x30, 0x2C, 0x24, 0x17C };
            Strength = BitConverter.ToInt32(ReadMemory((IntPtr)ReadPointer(StrengthAddress, StrengthOffset), (uint)4), 0);

            Int32 WGIDAddress = BaseAddress + 0x034084A0;
            Int32[] WGIDOffset = { 0x18, 0x18, 0x114, 0x28, 0x18, 0x2C, 0x30 };
            WGID = BitConverter.ToInt32(ReadMemory((IntPtr)ReadPointer(WGIDAddress, WGIDOffset), (uint)4), 0);
        }

        /// <summary>
        /// Damage Dealt : Нанесенный ущерб
        /// </summary>
        public int Dealt
        {
            get
            {
                return _dealt;
            }
            set
            {
                _dealt = value;
                _del?.Invoke();
            }
        }

        /// <summary>
        /// Damage Received : Полученный ущерб
        /// </summary>
        public int Received
        {
            get
            {
                return _received;
            }
            set
            {
                _received = value;
                _del?.Invoke();
            }
        }

        /// <summary>
        /// Strength Points : Очки прчности
        /// </summary>
        public int Strength
        {
            get
            {
                return _strength;
            }
            set
            {
                _strength = value;
                _del?.Invoke();
            }
        }

        /// <summary>
        /// WG User ID
        /// </summary>
        public int WGID
        {
            get
            {
                return _wgid;
            }
            set
            {
                _wgid = value;
                _del?.Invoke();
            }
        }

        private int _dealt;
        private int _received;
        private int _strength;
        private int _wgid;

        public int BaseAddress;
        private string pName;
        public int pID;

        #region Библиотеки
        [Flags]
        private enum SnapshotFlags : uint
        {
            HeapList = 0x00000001,
            Process = 0x00000002,
            Thread = 0x00000004,
            Module = 0x00000008,
            Module32 = 0x00000010,
            Inherit = 0x80000000,
            All = 0x0000001F,
            NoHeaps = 0x40000000
        }

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            const int MAX_PATH = 260;
            internal UInt32 dwSize;
            internal UInt32 cntUsage;
            internal UInt32 th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal UInt32 th32ModuleID;
            internal UInt32 cntThreads;
            internal UInt32 th32ParentProcessID;
            internal Int32 pcPriClassBase;
            internal UInt32 dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            internal string szExeFile;
        }

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr CreateToolhelp32Snapshot([In] UInt32 dwFlags, [In] UInt32 th32ProcessID);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32First([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern bool Process32Next([In] IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle([In] IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] buffer, UInt32 size, out IntPtr lpNumberofBytesRead);
        #endregion
    }
}
