namespace Prototype.Battle
{
    [System.Flags]
    public enum Element : byte 
    {
        none    = 0b_0000_0000,
        fire    = 0b_0000_0001,
        ice     = 0b_0000_0010,
        volt    = 0b_0000_0100
        //This is a test into using enums instead of strings for elements in the future
    }
}