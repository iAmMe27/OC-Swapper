using System;
using System.ComponentModel;
using System.IO;
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
            _lastRuntimeUsed = value ?? -1;
            OnPropertyChange(nameof(LastRuntimeUsed));
        }
    }

    public int? AuthorWindowAvailable
    {
        get => _authorWindowAvailable;
        set
        {
            _authorWindowAvailable = value ?? -1;
            OnPropertyChange(nameof(_authorWindowAvailable));
        }
    }
    
    private string _steamFileHash = string.Empty;
    private string _openCompositeFileHash = string.Empty;
    private string _openVrDllFilePath = string.Empty;
    private string _steamVrStorageFolder = string.Empty;
    private string _openCompositeStorageFolder = string.Empty;
    private int? _lastRuntimeUsed = 0;
    private int? _authorWindowAvailable = 0;

    private const string DllFileName = "openvr_api.dll";

    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void SettingsInitialiser()
    {
        var config = Configuration.LoadConfig();
        
        SteamFileHash = config?.SteamFileHash;
        OpenCompositeFileHash = config?.OpenCompositeFileHash;
        OpenVrDllFilePath = Path.Combine(AppContext.BaseDirectory, config!.OpenVrDllFilePath);
        SteamVrStorageFolder = Path.Combine(AppContext.BaseDirectory, config!.SteamVrStorageFolder, DllFileName);
        OpenCompositeStorageFolder = Path.Combine(AppContext.BaseDirectory, config!.OpenCompositeStorageFolder, DllFileName);
        LastRuntimeUsed = config?.LastRuntimeUsed;
        AuthorWindowAvailable = config?.AuthorWindowAvailable;
    }

    private void SaveConfig()
    {
        // Only save the last used runtime as the rest of the config is not written 
        // inside the program, only read
        
        SwapperConfig config = new()
        {
            LastRuntimeUsed = _lastRuntimeUsed ?? -1
        };

        Configuration.SaveConfig(config);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChange(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SettingsInitialiser();
        
        try
        {
            var currDll = DllCompare.Compare(_openVrDllFilePath, _steamFileHash);

            _lastRuntimeUsed = currDll switch
            {
                -1 => throw new FileNotFoundException(),
                0 => 0,
                1 => 1,
                _ => _lastRuntimeUsed
            };
        }
        catch (FileNotFoundException)
        {
            try
            {
                File.Copy(_steamVrStorageFolder, _openVrDllFilePath);
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
    
    // Instead of doing crap like this, I should really be using WPF XML bindings
    // TODO: use WPF XML bindings dang it
    private void UpdateUi()
    {
        switch (_lastRuntimeUsed)
        {
            case 0:
                LblCurrentBinaries.Content = "You are currently using OpenComposite binaries";
                BtnSwapBinaries.Content = "Swap to SteamVR";
                break;
            case 1:
                LblCurrentBinaries.Content = "You are currently using SteamVR binaries";
                BtnSwapBinaries.Content = "Swap to OpenComposite";
                break;
        }
    }

    private void BtnSwapBinaries_Click(object sender, RoutedEventArgs e)
    {
        // double check which binary we have, for sanity’s sake
        try
        {
            var currDll = DllCompare.Compare(_openVrDllFilePath, _steamFileHash);

            _lastRuntimeUsed = currDll switch
            {
                -1 => throw new FileNotFoundException(),
                0 => 0,
                1 => 1,
                _ => _lastRuntimeUsed
            };

            if (currDll != _lastRuntimeUsed)
            {
                MessageBox.Show("WARNING: Something has changed the openvr_api.dll binary whilst this program has been open! Something might have messed with the file when it shouldn't have.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    // copy in the OpenComposite file, last variable allows for overwrite
                    File.Copy(_openCompositeStorageFolder, _openVrDllFilePath, true);

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
                    // copy in the SteamVR file, last variable allows for overwrite
                    File.Copy(_steamVrStorageFolder, _openVrDllFilePath, true);

                    _lastRuntimeUsed = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error moving the openvr_api.dll file: " + ex.Message);
                }

                break;
        }

        SaveConfig();
        UpdateUi();
    }

    private void BtnAbout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("OC Swapper - a tool by iAmMe\n\nOC Swapper offers a single click solution for swapping between SteamVR DLL binary and OpenComposite DLL binary for Skyrim VR setups", "About OC Swapper");
    }

    private void BtnAuthorSettings_Click(object sender, RoutedEventArgs e)
    {
        var authorSettingsWindow = new AuthorSettings
        {
            Config = new SwapperConfig()
            {
                SteamFileHash = _steamFileHash,
                OpenCompositeFileHash = _openCompositeFileHash,
                OpenVrDllFilePath = _openVrDllFilePath,
                SteamVrStorageFolder = _steamVrStorageFolder,
                OpenCompositeStorageFolder = _openCompositeStorageFolder,
                LastRuntimeUsed = _lastRuntimeUsed ?? 0,
                AuthorWindowAvailable = _authorWindowAvailable ?? 0
            }
        };

        authorSettingsWindow.ShowDialog();
    }
}