using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdonisUI;
using System.IO;
using AdonisUI.Controls;
using IWshRuntimeLibrary;
using Forms = System.Windows.Forms;
using AutoUpdaterDotNET;
using System.Reflection;

namespace SpotifyVolumeManager
{
    /// <summary>
    /// Logica di interazione per HotKeySet.xaml
    /// </summary>
    public partial class HotKeySet : AdonisUI.Controls.AdonisWindow
    {
        public bool IsDark
        {
            get => (bool)GetValue(IsDarkProperty);
            set => SetValue(IsDarkProperty, value);
        }

        public static readonly DependencyProperty IsDarkProperty = DependencyProperty.Register("IsDark", typeof(bool), typeof(HotKeySet), new PropertyMetadata(false, OnIsDarkChanged));

        private static void OnIsDarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HotKeySet)d).ChangeTheme((bool)e.OldValue);
        }

        public HotKeySet()
        {
            InitializeComponent();
        }

        private void ChangeTheme(bool oldValue)
        {
            ResourceLocator.SetColorScheme(Application.Current.Resources, oldValue ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);
        }

        private void bootOnStartupSwitch_Checked(object sender, RoutedEventArgs e)
        {
            WshShell wshShell = new WshShell();
            IWshRuntimeLibrary.IWshShortcut shortcut;
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            // Create the shortcut
            shortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(startUpFolderPath + "\\" + Forms.Application.ProductName + ".lnk");

            shortcut.TargetPath = AppDomain.CurrentDomain.BaseDirectory+"\\"+ Forms.Application.ProductName + ".exe";
            shortcut.WorkingDirectory = Forms.Application.StartupPath;
            shortcut.Description = "HKS";
            shortcut.Save();
        }

        private void bootOnStartupSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            System.IO.File.Delete(System.IO.Path.Combine(startUpFolderPath, Forms.Application.ProductName + ".lnk"));
        }

        private void hksSettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Forms.Application.ProductName +".lnk";
            if (System.IO.File.Exists(filePath))
                bootOnStartupSwitch.IsChecked = true;
        }
    }
}
