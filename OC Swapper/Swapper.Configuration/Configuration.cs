using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using OC_Swapper.Swapper.Json;

namespace OC_Swapper.Swapper.Configuration;

public static class Configuration
{
    private const string ConfigFile = "config.json";

    public static async Task<SwapperConfig?> LoadConfig()
    {
        SwapperConfig? config;

        try
        {
            config = await JsonInterface.Read<SwapperConfig>(ConfigFile);
        }
        catch (Exception)
        {
            config = await CreateNewSettings();
            MessageBox.Show("Could not read configuration file, a new one has been created. You will need to populate it with the correct settings.");
        }

        return config;
    }

    public static async Task SaveConfig(SwapperConfig config)
    {
        await JsonInterface.Write(ConfigFile, config);
    }

    private static async Task<SwapperConfig> CreateNewSettings()
    {
        SwapperConfig config = new()
        {
            SteamFileHash = "",
            OpenCompositeFileHash = "",
            OpenVrDllFilePath = "",
            SteamVrStorageFolder = "",
            OpenCompositeStorageFolder = "",
            LastRuntimeUsed = 0
        };
        
        await SaveConfig(config);
        return config;
    }
}

public class SwapperConfig
{
    [JsonPropertyName("SteamFileHash")]
    public string SteamFileHash { get; set; } = string.Empty;
        
    [JsonPropertyName("OpenCompositeFileHash")]
    public string OpenCompositeFileHash { get; set; } = string.Empty;
        
    [JsonPropertyName("OpenVRDLLFilePath")]
    public string OpenVrDllFilePath { get; set; } = string.Empty;

    [JsonPropertyName("SteamVRStorageFolder")]
    public string SteamVrStorageFolder { get; set; } = string.Empty;
        
    [JsonPropertyName("OpenCompositeStorageFolder")]
    public string OpenCompositeStorageFolder { get; set; } = string.Empty;

    [JsonPropertyName("LastUsed")] 
    public int LastRuntimeUsed { get; set; } = 0;
}