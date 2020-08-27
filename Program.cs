using System;

namespace NFL_Street_Memory_Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            DolphinProcessInfo dolphin_Process = new DolphinProcessInfo();
            if (dolphin_Process.InitializeDolphinInfo())
            {
                Console.WriteLine(dolphin_Process.MemoryStartAddress.ToString("X8"));
            }
            else 
            {
                Console.WriteLine("Unable to find Dolphin's base address...");
            }
        }
    }
}
