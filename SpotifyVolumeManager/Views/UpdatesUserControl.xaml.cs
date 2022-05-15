using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Mime;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoUpdaterDotNET;
using SpotifyVolumeManager.Models;
using Forms = System.Windows.Forms;

namespace SpotifyVolumeManager.Views
{
    /// <summary>
    /// Logica di interazione per UpdatesUserControl.xaml
    /// </summary>
    public partial class UpdatesUserControl : UserControl, Models.IProgressBarChangeListener
    {
        AdonisUI.Controls.MessageBoxModel messageBox;
        AdonisUI.Controls.MessageBoxResult resultMessageBox;
        public UpdatesUserControl()
        {
            InitializeComponent();
            Models.AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdaterDotNET.DownloadUpdateDialog.AddListener(this);
        }

        private void updatesUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            hksVersionUpdateTextBlock.Text += FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString()).FileVersion.ToString();
            if (Properties.Settings.Default.autoUpdate.Equals("True"))
                autoUpdateSwitch.IsChecked = true;
            else
                autoUpdateSwitch.IsChecked = false;
        }

        private void autoUpdateSwitch_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.autoUpdate = "True";
            Properties.Settings.Default.Save();   
        }

        private void autoUpdateSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.autoUpdate = "False";
            Properties.Settings.Default.Save();
        }

        private void checkUpdatesButton_Click(object sender, RoutedEventArgs e)
        { 
            Models.AutoUpdater.Start("https://raw.githubusercontent.com/UranusDarkness/HKS/gh-pages/src/autoUpdate.xml");
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    //Forms.DialogResult dialogResult;
                    if (args.Mandatory.Value)
                    {
                        /*dialogResult =
                            Forms.MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. This is required update. Press Ok to begin updating the application.", @"Update Available",
                                Forms.MessageBoxButtons.OK,
                                Forms.MessageBoxIcon.Information);*/
                        resultMessageBox = AdonisUI.Controls.MessageBox.Show($@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. This is required update. Press Ok to begin updating the application.", @"Update Available", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);
                    }
                    else
                    {
                        /*dialogResult =
                            Forms.MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {
                                        args.InstalledVersion
                                    }. Do you want to update the application now?", @"Update Available",
                                Forms.MessageBoxButtons.YesNo,
                                Forms.MessageBoxIcon.Information);*/
                        resultMessageBox = AdonisUI.Controls.MessageBox.Show($@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. Do you want to update the application now?", @"Update Available", AdonisUI.Controls.MessageBoxButton.YesNo, AdonisUI.Controls.MessageBoxImage.Information);
                    }

                    // Uncomment the following line if you want to show standard update dialog instead.
                    //AutoUpdater.ShowUpdateForm(args);

                    //if (dialogResult.Equals(Forms.DialogResult.Yes) || dialogResult.Equals(Forms.DialogResult.OK))
                    if (resultMessageBox.Equals(AdonisUI.Controls.MessageBoxResult.Yes) || resultMessageBox.Equals(AdonisUI.Controls.MessageBoxResult.OK))
                    {
                        try
                        {
                            if (Models.AutoUpdater.DownloadUpdate(args))
                            {
                                
                            }

                        }
                        catch (Exception exception)
                        {
                            /*Forms.MessageBox.Show(exception.Message, exception.GetType().ToString(), Forms.MessageBoxButtons.OK,
                                Forms.MessageBoxIcon.Error);*/
                            AdonisUI.Controls.MessageBox.Show(exception.Message, exception.GetType().ToString(), AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    /*Forms.MessageBox.Show(@"There is no update available please try again later.", @"No update available",
                        Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Information);*/
                    AdonisUI.Controls.MessageBox.Show(@"There is no update available please try again later.", @"No update available", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Information);
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    /*Forms.MessageBox.Show(
                        @"There is a problem reaching update server. Please check your internet connection and try again later.",
                        @"Update Check Failed", Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Error);*/
                    AdonisUI.Controls.MessageBox.Show(@"There is a problem reaching update server. Please check your internet connection and try again later.",
                        @"Update Check Failed", AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                }
                else
                {
                    /*Forms.MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), Forms.MessageBoxButtons.OK,
                        Forms.MessageBoxIcon.Error);*/
                    AdonisUI.Controls.MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), AdonisUI.Controls.MessageBoxButton.OK, AdonisUI.Controls.MessageBoxImage.Error);
                }
            }
            
        }

        public void ProgressBarChange(int progress)
        {
            updateProgressBar.Value = progress;
        }

        public void DownloadCompleted()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
