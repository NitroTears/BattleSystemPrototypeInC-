namespace Prototype.Battle
{
    [System.Flags]
    public enum Element : byte 
    {
        None    = 0b_0000_0000,
        Fire    = 0b_0000_0001,
        Ice     = 0b_0000_0010,
        Volt    = 0b_0000_0100
        //This is a test into using enums instead of strings for elements in the future
    }
}