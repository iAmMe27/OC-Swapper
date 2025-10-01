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

            //return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return Convert.ToHexStringLower(hash).Replace("-", "");
        }
        catch (Exception ex) 
        {
            MessageBox.Show("[HASH] Error hashing file: " + ex.Message);
            return string.Empty;
        }
    }
}