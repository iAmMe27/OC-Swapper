using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;

namespace OC_Swapper.Swapper.Files;

public static class Hash
{
    public static string GetFileHash(string? filePath)
    {
        try
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filePath ?? throw new InvalidOperationException());
            var hash = md5.ComputeHash(stream);

            return Convert.ToHexStringLower(hash).Replace("-", "");
        }
        catch (Exception) 
        {
            return string.Empty;
        }
    }
}