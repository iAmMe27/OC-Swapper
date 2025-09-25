using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;

namespace OC_Swapper.Swapper.Files;

public static class Hash
{
    public static async Task<string?> GetFileHash(string? filePath)
    {
        try
        {
            using var md5 = MD5.Create();
            await using var stream = File.OpenRead(filePath ?? throw new InvalidOperationException());
            var hash = await md5.ComputeHashAsync(stream);

            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
        catch (Exception ex) 
        {
            MessageBox.Show("Error hashing file: " + ex.Message);
            return null;
        }
    }
}