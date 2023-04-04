using System;
using System.IO;
using System.Security.Cryptography;
using System.Windows;

using IniParser;
using IniParser.Model;

namespace OC_Swapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {       
        private int CurrentlyUsedBinaries = 0; // 0 = SteamVR files, 1 = OpenComposite files

        IniData ConfigData = new();
        private readonly string ConfigFile = "config.ini";
        //private readonly string SteamVRStorageFolder = "SteamVR Files/openvr_api.dll";
        //private readonly string OpenCompositeStorageFolder = "OpenComposite Files/openvr_api.dll";
        private string OpenVRDLLFileHash = string.Empty;

        ConfigurationInfo ConfigInfo = new ConfigurationInfo();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // On form load (so first start), check for the config.ini file
            if (File.Exists(ConfigFile))
            {
                // it does, read it
                var parser = new FileIniDataParser();

                try
                {
                    ConfigData = parser.ReadFile(ConfigFile);

                    // assign values
                    ConfigInfo.SteamFileHash = ConfigData["FILES"]["SteamFile"];
                    ConfigInfo.OpenCompositeFileHash = ConfigData["FILES"]["OpenCompositeFile"];
                    ConfigInfo.OpenVRDLLFilePath = ConfigData["FILES"]["OpenVRDLLFilePath"] + "\\openvr_api.dll";
                    ConfigInfo.SteamVRStorageFolder = ConfigData["FILES"]["SteamVRStorageFolder"] + "\\openvr_api.dll";
                    ConfigInfo.OpenCompositeStorageFolder = ConfigData["FILES"]["OpenCompositeStorageFolder"] + "\\openvr_api.dll";

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error reading config file: " + ex.Message.ToString());
                }

                // attempt to hash the openvr_api.dll file and work out if its Steam or OpenComposite
                try
                {
                    OpenVRDLLFileHash = GetFileHash(ConfigInfo.OpenVRDLLFilePath);

                    if (Equals(ConfigInfo.SteamFileHash, OpenVRDLLFileHash))
                    {
                        // if the strings match, we have the SteamVR file enabled
                        CurrentlyUsedBinaries = 0;
                    }
                    else if (Equals(ConfigInfo.OpenCompositeFileHash, OpenVRDLLFileHash))
                    {
                        // if the strings match here, we have the OpenComposite file enabled
                        CurrentlyUsedBinaries = 1;
                    }
                }
                catch (FileNotFoundException)
                {
                    // openvr_api.dll is not there for some reason, default to copying the SteamVR file
                    try
                    {
                        // then copy in the OpenComposite file
                        File.Copy(ConfigInfo.SteamVRStorageFolder, ConfigInfo.OpenVRDLLFilePath);

                        CurrentlyUsedBinaries = 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error moving the openvr_api.dll file: " + ex.Message.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error hashing openvr_api.dll file: " + ex.Message.ToString());
                }
            }
            else
            {
                // it does not, make it

                // files section
                ConfigData["FILES"]["SteamFile"] = "";
                ConfigData["FILES"]["OpenCompositeFile"] = "";
                ConfigData["FILES"]["OpenVRDLLFilePath"] = "";
                ConfigData["FILES"]["SteamVRStorageFolder"] = "SteamVR Files";
                ConfigData["FILES"]["OpenCompositeStorageFolder"] = "OpenComposite Files";

                // runtime section
                ConfigData["RUNTIME"]["LastUsed"] = "0";

                // save the file
                var parser = new FileIniDataParser();

                try
                {
                    parser.WriteFile(ConfigFile, ConfigData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error creating config file: " + ex.Message.ToString());
                }
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            // change label text based on the currently selected binaries
            if (CurrentlyUsedBinaries == 0)
            {
                LblCurrentBinaries.Content = "You are currently using SteamVR binaries";
                BtnSwapBinaries.Content = "Swap to OpenComposite";
            }
            else if (CurrentlyUsedBinaries == 1)
            {
                LblCurrentBinaries.Content = "You are currently using OpenComposite binaries";
                BtnSwapBinaries.Content = "Swap to SteamVR";
            }
        }

        private static string GetFileHash(string FilePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var Stream = File.OpenRead(@FilePath))
                {
                    var hash = md5.ComputeHash(Stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void BtnSwapBinaries_Click(object sender, RoutedEventArgs e)
        {
            // double check which binary we have, for sanity sake
            try
            {
                string CurrentOpenVRFileHash = GetFileHash(ConfigInfo.OpenVRDLLFilePath);
                int CurrentOpenVRType = 0; // 0 for SteamVR files, 1 for OpenComposite files

                if (Equals(ConfigInfo.SteamFileHash, CurrentOpenVRFileHash))
                {
                    // if the strings match, we have the SteamVR file enabled
                    CurrentOpenVRType = 0;
                }
                else if (Equals(ConfigInfo.OpenCompositeFileHash, CurrentOpenVRFileHash))
                {
                    // if the strings match here, we have the OpenComposite file enabled
                    CurrentOpenVRType = 1;
                }

                if (CurrentOpenVRType != CurrentlyUsedBinaries)
                {
                    MessageBox.Show("WARNNING: Something has changed the openvr_api.dll binaries whilst this program has been open! Something might have messed with the file when it shouldn't have.", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying the current openvr_api.dll file: " + ex.Message.ToString());
            }

            // move files as necessary
            if (CurrentlyUsedBinaries == 0) // SteamVR files
            {
                try
                {
                    // remove current file
                    File.Delete(ConfigInfo.OpenVRDLLFilePath);

                    // then copy in the OpenComposite file
                    File.Copy(ConfigInfo.OpenCompositeStorageFolder, ConfigInfo.OpenVRDLLFilePath);

                    CurrentlyUsedBinaries = 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error moving the openvr_api.dll file: " + ex.Message.ToString());
                }
            }
            else if (CurrentlyUsedBinaries == 1) // OpenComposite files
            {
                try
                {
                    // remove current file
                    File.Delete(ConfigInfo.OpenVRDLLFilePath);

                    // then copy in the OpenComposite file
                    File.Copy(ConfigInfo.SteamVRStorageFolder, ConfigInfo.OpenVRDLLFilePath);

                    CurrentlyUsedBinaries = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error moving the openvr_api.dll file: " + ex.Message.ToString());
                }
            }

            UpdateUI();
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("OC Swapper - a tool by iAmMe\n\nOC Swapper offers a single click solution for swapping between SteamVR DLL binary and OpenComposite DLL binary for Skyrim VR setups", "About OC Swapper");
        }
    }

    public class ConfigurationInfo
    {
        private string SteamVRDLLHash, OpenCompositeDLLHash, DLLFilePath, SteamVRStoragePath, OpenCompositeStoragePath = string.Empty;

        public string SteamFileHash
        {
            get { return SteamVRDLLHash; }
            set { SteamVRDLLHash = value; }
        }

        public string OpenCompositeFileHash
        {
            get { return OpenCompositeDLLHash; }
            set { OpenCompositeDLLHash = value; }
        }

        public string OpenVRDLLFilePath
        {
            get { return DLLFilePath; }
            set { DLLFilePath = value; }
        }

        public string SteamVRStorageFolder
        {
            get { return SteamVRStoragePath; }
            set { SteamVRStoragePath = value; }
        }

        public string OpenCompositeStorageFolder
        {
            get { return OpenCompositeStoragePath; }
            set { OpenCompositeStoragePath= value; }
        }
    }
}
