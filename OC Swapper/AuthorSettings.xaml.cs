using System;
using System.Windows;
using Microsoft.Win32;

using OC_Swapper.Swapper.Configuration;
using OC_Swapper.Swapper.Files;

namespace OC_Swapper;

/// <summary>
/// Interaction logic for AuthorSettings.xaml
/// </summary>
public partial class AuthorSettings : Window
{
    public SwapperConfig? Config;
    
    public AuthorSettings()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        TxtOpenCompositeStorageFolder.Text = Config?.OpenCompositeStorageFolder ?? "...";
        TxtOpenCompositeStorageFolder.ScrollToEnd();
        
        TxtSteamVrStorageFolder.Text = Config?.SteamVrStorageFolder ?? "...";
        TxtSteamVrStorageFolder.ScrollToEnd();
        
        TxtOpenVrDllFilePath.Text = Config?.OpenVrDllFilePath ?? "...";
        TxtOpenVrDllFilePath.ScrollToEnd();
    }

    private static string ShowFileDialog(string initialDirectory, string filter)
    {
        var openFileDialog = new OpenFileDialog
        {
            InitialDirectory = initialDirectory,
            Filter = filter
        };
    
        openFileDialog.ShowDialog();
        return openFileDialog.FileName;
    }

    private static string ShowFolderDialog(string initialDirectory)
    {
        var openFolderDialog = new OpenFolderDialog
        {
            InitialDirectory = initialDirectory
        };
        
        openFolderDialog.ShowDialog();
        return openFolderDialog.FolderName;
    }

    private void BtnOpenCompositeFileLocation_OnClick(object sender, RoutedEventArgs e)
    {
        if (Config == null) return;
        Config.OpenCompositeStorageFolder = ShowFolderDialog(Config.OpenCompositeStorageFolder);
        TxtOpenCompositeStorageFolder.Text = Config.OpenCompositeStorageFolder;
    }

    private void BtnSteamVrStorageFolder_OnClick(object sender, RoutedEventArgs e)
    {
        if (Config == null) return;
        Config.SteamVrStorageFolder = ShowFolderDialog(Config.SteamVrStorageFolder);
        TxtSteamVrStorageFolder.Text = Config.SteamVrStorageFolder;
    }

    private void BtnOpenVrDllFilePath_OnClick(object sender, RoutedEventArgs e)
    {
        if (Config == null) return;
        Config.OpenVrDllFilePath = ShowFileDialog(Config.OpenVrDllFilePath, "openvr_api.dll (openvr_api.dll)|openvr_api.dll");
        TxtOpenVrDllFilePath.Text = Config.OpenVrDllFilePath;
    }
}