using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class DolphinProcessInfo {
    
    #region Properties

    public IntPtr ProcessPointer
    {
        get;
        private set;
    }

    public IntPtr MemoryStartAddress 
    { 
        get; 
        private set;
    }

    #endregion
    
    #region Methods

    public bool InitializeDolphinInfo()
    {
        // Get the Dolphin process
        Process[] processList = Process.GetProcessesByName("dolphin");
        // TODO: Add better handling for when Dolphin isn't running
        if (processList.Length == 0)
        {
            return false;
        }
        Process process = processList[0];
        ProcessPointer = WindowsSystemUtils.OpenProcess(0x0400 | 0x0008 | 0x0020 | 0x0010, false, process.Id);

        WindowsSystemUtils.SYSTEM_INFO sys_info = new WindowsSystemUtils.SYSTEM_INFO();
        WindowsSystemUtils.GetSystemInfo(out sys_info);
        Int64 maxAddress = (Int64)sys_info.maximumApplicationAddress;
        long currentAddress = 0;

        do
        {
            WindowsSystemUtils.MEMORY_BASIC_INFORMATION64 memoryInfo;
            int result = WindowsSystemUtils.VirtualQueryEx(ProcessPointer, (IntPtr)currentAddress, out memoryInfo, (uint)Marshal.SizeOf(typeof(WindowsSystemUtils.MEMORY_BASIC_INFORMATION64)));

            // We are looking for MEM_MAPPED (0x40000) memory of size 0x2000000
            if ((int)memoryInfo.RegionSize == 0x2000000 && memoryInfo.Type == 0x40000)
            {
                // Confirm that the current page has valid working set information, otherwise ignore it
                WindowsSystemUtils._PSAPI_WORKING_SET_EX_INFORMATION[] WsInfo = new WindowsSystemUtils._PSAPI_WORKING_SET_EX_INFORMATION [1];
                WsInfo[0].VirtualAddress = (IntPtr)memoryInfo.BaseAddress;
                if (WindowsSystemUtils.QueryWorkingSetEx(ProcessPointer, WsInfo, Marshal.SizeOf<WindowsSystemUtils._PSAPI_WORKING_SET_EX_INFORMATION>()))
                {
                    Console.WriteLine(WsInfo[0].VirtualAttributes.Flags.ToString("X8"));
                    // Checks the Valid flag on the PSAPI response
                    if ((WsInfo[0].VirtualAttributes.Flags & 0b1) == 1)
                    {
                        if (MemoryStartAddress == IntPtr.Zero) 
                        {
                            // TODO: Add handling for multiple pages
                            MemoryStartAddress = WsInfo[0].VirtualAddress;
                        }
                    }
                }
            }

            // Check to see if we've searched the entire process memory region
            if (currentAddress == (long)memoryInfo.BaseAddress + (long)memoryInfo.RegionSize)
            {
                break;
            }

            // Jump to the next page
            currentAddress = (long)memoryInfo.BaseAddress + (long)memoryInfo.RegionSize;

        } while (currentAddress <= (long)maxAddress);
        
        return true;
    }

    #endregion

}

