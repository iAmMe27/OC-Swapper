using System;
using System.Windows;
using Microsoft.Win32;
using OC_Swapper.Swapper.Configuration;

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

    public string ShowFileDialog(string initialDirectory, string filter)
    {
        var openFileDialog = new OpenFileDialog
        {
            InitialDirectory = initialDirectory,
            Filter = filter
        };

        return openFileDialog.FileName;
    }
}