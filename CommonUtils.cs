using System;

public static class CommonUtils
{
    public static UInt32 DolphinAddressToOffset(UInt32 address)
    {
        return address &= 0x7FFFFFFF; 
    }

    public static UInt32 OffsetToDolphinAddress(UInt32 offset)
    {
        return offset |= 0x80000000; 
    }
}
