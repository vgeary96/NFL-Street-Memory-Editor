using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace NFL_Street_Memory_Editor
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            DolphinProcessInfo dolphin_Process = new DolphinProcessInfo();
            // int[] attributes = { 19, 19, 1, 8, 5, 16, 17, 6, 6, 2 };
            // Player Lamar = new Player("Thomas", "Bullock", 8, "QB", 9, 212, attributes);
            if (dolphin_Process.InitializeDolphinInfo())
            {
                Console.WriteLine("Process base address: " + dolphin_Process.MemoryStartAddress.ToString("X8"));
                int index = 0;
                List<Player> players = new List<Player>();
                using (var reader = new StreamReader(@"C:\Users\Victor Geary\Desktop\Street NFC - 2020\Street Rosters_vSEA.csv"))
                {

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        Console.WriteLine(string.Join(",", values));

                        string[] attributeStrings = new string[10];
                        Array.Copy(values, 6, attributeStrings, 0, attributeStrings.Length);
                        int[] attributes = Array.ConvertAll(attributeStrings, s => int.Parse(s));

                        players.Add(new Player(index, values[0], values[1], Int32.Parse(values[2]), values[3], Int32.Parse(values[4]), Int32.Parse(values[5]), attributes));
                        index++;
                    }
                }

                IntPtr ptr = new IntPtr(dolphin_Process.MemoryStartAddress.ToInt64() + 2152221376);
                byte[] consoleAddressBuffer = new byte[(sizeof(byte) * 88)];

                // Write the player count
                IntPtr playerCountAddress = (IntPtr)(dolphin_Process.MemoryStartAddress.ToInt64() + CommonUtils.DolphinAddressToOffset(0x80484691));
                byte[] playerCount = { (byte)players.Count };
                WindowsSystemUtils.WriteProcessMemory(dolphin_Process.ProcessPointer, playerCountAddress, playerCount, sizeof(byte), out _);


                // Loop over the players
                IntPtr playerAddress = (IntPtr)(dolphin_Process.MemoryStartAddress.ToInt64() + CommonUtils.DolphinAddressToOffset(0x80484ac0));
                for (int i = 0; i < players.Count; i++)
                {
                    WindowsSystemUtils.WriteProcessMemory(dolphin_Process.ProcessPointer, playerAddress, players[i].GetByteStream(), (sizeof(byte) * 88), out _);
                    playerAddress += 88;
                }
            }
            else
            {
                Console.WriteLine("Unable to find Dolphin's base address...");
            }
        }
    }
}
