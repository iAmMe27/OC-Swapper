using System;

namespace OC_Swapper.Swapper.Files;

public static class DllCompare
{
    public static int Compare(string? dllToCompare, string? hashToCompareTo)
    {
        try
        {
            var currentFileHash = Hash.GetFileHash(dllToCompare);

            if (currentFileHash == string.Empty)
            {
                return -1;
            }
            
            return currentFileHash == hashToCompareTo ? 1 : 0;
        }
        catch (Exception)
        {
            return -1;
        }
    }
}    

