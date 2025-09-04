using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

using OC_Swapper.Swapper.Configuration;
using OC_Swapper.Swapper.Files;

namespace OC_Swapper;

public partial class MainWindow
{
    public string? SteamFileHash
    {
        get => _steamFileHash;
        set
        {
            _steamFileHash = value ?? string.Empty;
            OnPropertyChange(nameof(SteamFileHash));
        }
    }
    
    public string? OpenCompositeFileHash
    {
        get => _openCompositeFileHash;
        set
        {
            _openCompositeFileHash = value ?? string.Empty;
            OnPropertyChange(nameof(OpenCompositeFileHash));
        }
    }
    
    public string? OpenVrDllFilePath
    {
        get => _openVrDllFilePath;
        set
        {
            _openVrDllFilePath = value ?? string.Empty;
            OnPropertyChange(nameof(OpenVrDllFilePath));
        }
    }
    
    public string? SteamVrStorageFolder
    {
        get => _steamVrStorageFolder;
        set
        {
            _steamVrStorageFolder = value ?? string.Empty;
            OnPropertyChange(nameof(SteamVrStorageFolder));
        }
    }
    
    public string? OpenCompositeStorageFolder
    {
        get => _openCompositeStorageFolder;
        set
        {
            _openCompositeStorageFolder = value ?? string.Empty;
            OnPropertyChange(nameof(OpenCompositeStorageFolder));
        }
    }
    
    public int? LastRuntimeUsed
    {
        get => _lastRuntimeUsed;
        set
        {
            _lastRuntimeUsed = value ?? 0;
            OnPropertyChange(nameof(LastRuntimeUsed));
        }
    }
    
    private string? _steamFileHash = string.Empty;
    private string _openCompositeFileHash = string.Empty;
    private string _openVrDllFilePath = string.Empty;
    private string _steamVrStorageFolder = string.Empty;
    private string _openCompositeStorageFolder = string.Empty;
    private int? _lastRuntimeUsed = 0;

    public MainWindow()
    {
        InitializeComponent();

        _ = SettingsInitialiser();
    }
    
    private async Task SettingsInitialiser()
    {
        var config = await Configuration.LoadConfig();
        
        SteamFileHash = config?.SteamFileHash;
        OpenCompositeFileHash = config?.OpenCompositeFileHash;
        OpenVrDllFilePath = config?.OpenVrDllFilePath;
        SteamVrStorageFolder = config?.SteamVrStorageFolder;
        OpenCompositeStorageFolder = config?.OpenCompositeStorageFolder;
        LastRuntimeUsed = config?.LastRuntimeUsed;
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChange(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var currentFileHash = Hash.GetFileHash(OpenVrDllFilePath).Result;
                    
            if (Equals(SteamFileHash, currentFileHash))
            {
                _lastRuntimeUsed = 0;
            }
            else if (Equals(OpenCompositeFileHash, currentFileHash))
            {
                _lastRuntimeUsed = 1;
            }
        }
        catch (FileNotFoundException)
        {
            try
            {
                File.Copy(SteamVrStorageFolder ?? throw new InvalidOperationException(), OpenVrDllFilePath ?? throw new InvalidOperationException());
                _lastRuntimeUsed = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error moving the openvr_api.dll file: " + ex.Message);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error hashing openvr_api.dll file: " + ex.Message);
        }
            
        UpdateUi();
    }

    private void UpdateUi()
    {
        switch (_lastRuntimeUsed)
        {
            case 0:
                LblCurrentBinaries.Content = "You are currently using SteamVR binaries";
                BtnSwapBinaries.Content = "Swap to OpenComposite";
                break;
            case 1:
                LblCurrentBinaries.Content = "You are currently using OpenComposite binaries";
                BtnSwapBinaries.Content = "Swap to SteamVR";
                break;
        }
    }

    private void BtnSwapBinaries_Click(object sender, RoutedEventArgs e)
    {
        // double check which binary we have, for sanity’s sake
        try
        {
            var currentOpenVrFileHash = Hash.GetFileHash(OpenVrDllFilePath).Result;
            var currentOpenVrType = 0; // 0 for SteamVR files, 1 for OpenComposite files

            if (Equals(SteamFileHash, currentOpenVrFileHash))
            {
                // if the strings match, we have the SteamVR file enabled
                currentOpenVrType = 0;
            }
            else if (Equals(OpenCompositeFileHash, currentOpenVrFileHash))
            {
                // if the strings match here, we have the OpenComposite file enabled
                currentOpenVrType = 1;
            }

            if (currentOpenVrType != _lastRuntimeUsed)
            {
                MessageBox.Show("WARNING: Something has changed the openvr_api.dll binaries whilst this program has been open! Something might have messed with the file when it shouldn't have.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error verifying the current openvr_api.dll file: " + ex.Message);
        }

        switch (_lastRuntimeUsed)
        {
            // move files as necessary
            // SteamVR files
            case 0:
                try
                {
                    // remove current file
                    File.Delete(OpenVrDllFilePath ?? throw new InvalidOperationException());

                    // then copy in the OpenComposite file
                    File.Copy(OpenCompositeStorageFolder ?? throw new InvalidOperationException(), OpenVrDllFilePath);

                    _lastRuntimeUsed = 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error moving the openvr_api.dll file: " + ex.Message);
                }

                break;
            // OpenComposite files
            case 1:
                try
                {
                    // remove current file
                    File.Delete(OpenVrDllFilePath ?? throw new InvalidOperationException());

                    // then copy in the OpenComposite file
                    File.Copy(SteamVrStorageFolder ?? throw new InvalidOperationException(), OpenVrDllFilePath);

                    _lastRuntimeUsed = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error moving the openvr_api.dll file: " + ex.Message);
                }

                break;
        }
        
        UpdateUi();
    }

    private void BtnAbout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("OC Swapper - a tool by iAmMe\n\nOC Swapper offers a single click solution for swapping between SteamVR DLL binary and OpenComposite DLL binary for Skyrim VR setups", "About OC Swapper");
    }
}