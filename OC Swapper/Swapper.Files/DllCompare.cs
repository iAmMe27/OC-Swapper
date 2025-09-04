namespace OC_Swapper.Swapper.Files;

public static class DllCompare
{
    public static int Compare(string? dllToCompare, string? hashToCompareTo)
    {
        var currentFileHash = Hash.GetFileHash(dllToCompare).Result;
        return currentFileHash == hashToCompareTo ? 1 : 0;
    }
}    

