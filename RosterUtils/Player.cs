using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class Player
{

    #region Properties

    public string FirstName
    {
        get;
        private set;
    }
    // MAXLENGTH 11

    public string LastName
    {
        get;
        private set;
    }
    // MAXLENGTH 13

    public int Index
    {
        get;
        private set;
    }

    public int Number
    {
        get;
        private set;
    }

    public string Position
    {
        get;
        private set;
    }

    public int Height
    {
        get;
        private set;
    }

    public int Weight
    {
        get;
        private set;
    }

    public int Passing
    {
        get;
        private set;
    }

    public int Catching
    {
        get;
        private set;
    }

    public int Blocking
    {
        get;
        private set;
    }

    public int RunPower
    {
        get;
        private set;
    }

    public int Carrying
    {
        get;
        private set;
    }

    public int Agility
    {
        get;
        private set;
    }

    public int Speed
    {
        get;
        private set;
    }

    public int Tackling
    {
        get;
        private set;
    }

    public int Coverage
    {
        get;
        private set;
    }

    public int DMoves
    {
        get;
        private set;
    }

    #endregion

    #region Methods

    public Player(int index, string firstName, string lastName, int number, string position, int height, int weight, int[] attributes)
    {
        Index = index;
        FirstName = firstName;
        LastName = lastName;
        Number = number;
        Position = position;
        Height = height;
        Weight = weight;
        Console.WriteLine(attributes[1]);
        Passing = attributes[0];
        Catching = attributes[1];
        Blocking = attributes[2];
        RunPower = attributes[3];
        Carrying = attributes[4];
        Agility = attributes[5];
        Speed = attributes[6];
        Tackling = attributes[7];
        Coverage = attributes[8];
        DMoves = attributes[9];
    }

    public byte[] GetByteStream()
    {
        /// Index ///
        UInt16 indexU16 = (UInt16)(Index * 64);
        byte[] indexBytes = BitConverter.GetBytes(indexU16);

        // byte[] stream = new byte[sizeof(byte) * 88];
        byte[] stream = {68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 80, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 132, 32, 0, 0, 0, 0, 0, 15, 255, 224, 1, 0, 8, 16, 4, 8, 16, 0, 120, 10, 0, 33, 242, 48, 0, 0, 8, 0, 20, 2, 3, 5, 129, 49, 192, 2, 2, 64, 0, 0, 0};

        byte[] firstNameBytes = ASCIIEncoding.ASCII.GetBytes(FirstName);
        if (firstNameBytes.Length > 11)
        {
            Array.Resize(ref firstNameBytes, 11);
        }

        byte[] lastNameBytes = ASCIIEncoding.ASCII.GetBytes(LastName);
        if (lastNameBytes.Length > 13)
        {
            Array.Resize(ref lastNameBytes, 13);
        }

        Array.Copy(firstNameBytes, stream, firstNameBytes.Length);
        Array.Copy(lastNameBytes, 0, stream, 11, lastNameBytes.Length);

        /// Passing ///
        // stream[75] = Convert.ToByte(Passing * 10); // Might need to set it to 02 if it's 0
        /// Speed ///
        // stream[57] = Convert.ToByte(Speed * 5); // Might need to set it to 01 if it's 0
        /// Blocking ///
        UInt16 blockingU16 = (UInt16)(1024 + (Blocking * 40));
        byte[] blockingBytes = BitConverter.GetBytes(blockingU16);

        /// Agility ///
        UInt16 agilityU16 = (UInt16)((Agility * 80));
        byte[] agilityBytes = BitConverter.GetBytes(agilityU16);

        /// Catching ///
        UInt16 catchingU16 = (UInt16)((Catching * 40));
        byte[] catchingBytes = BitConverter.GetBytes(catchingU16);

        /// Run Power /// 
        UInt16 runPowerU16 = (UInt16)((RunPower * 80));
        byte[] runPowerBytes = BitConverter.GetBytes(runPowerU16);

        /// Carrying /// 
        UInt16 carryingU16 = (UInt16)((Carrying * 40));
        byte[] carryingBytes = BitConverter.GetBytes(carryingU16);

        /// Tackling /// 
        UInt16 tacklingU16 = (UInt16)((Tackling * 20));
        byte[] tacklingBytes = BitConverter.GetBytes(tacklingU16);

        /// Coverage /// 
        UInt16 coverageU16 = (UInt16)((Coverage * 10));
        byte[] coverageBytes = BitConverter.GetBytes(coverageU16);

        /// D-Moves /// 
        UInt16 dMovesU16 = (UInt16)((DMoves * 10) + 1);
        byte[] dMovesBytes = BitConverter.GetBytes(dMovesU16);

        Console.WriteLine(BitConverter.ToString(indexBytes));
        stream[52] = indexBytes[1];
        stream[53] = indexBytes[0];

        stream[57] = Convert.ToByte(Speed * 5); // Might need to set it to 01 if it's 0
        stream[58] = catchingBytes[1];
        stream[59] = (byte)(agilityBytes[1] + catchingBytes[0]);
        stream[60] = (byte)(agilityBytes[0] + tacklingBytes[1]);
        stream[61] = (byte)(tacklingBytes[0] + blockingBytes[1]); // These are given to us in reverse order...
        stream[62] = (byte)(blockingBytes[0] + runPowerBytes[1]);
        stream[63] = runPowerBytes[0];

        stream[72] = carryingBytes[1];
        stream[73] = carryingBytes[0];

        stream[75] = Convert.ToByte(Passing * 10); // Might need to set it to 02 if it's 0


        stream[77] = dMovesBytes[0];

        stream[82] = coverageBytes[0];
        return stream;
    }

    #endregion

}

