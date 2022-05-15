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
using AdonisUI.Controls;
using AdonisUI;

namespace SpotifyVolumeManager
{
    /// <summary>
    /// Logica di interazione per AutoUpdateWindow.xaml
    /// </summary>
    public partial class AutoUpdateWindow : AdonisUI.Controls.AdonisWindow, Models.IProgressBarChangeListener
    {
        public AutoUpdateWindow()
        {
            InitializeComponent();
            AutoUpdaterDotNET.DownloadUpdateDialog.AddListener(this);
            ChangeTheme(Properties.Settings.Default.colorMode);
        }

        public bool IsDark;

        private void ChangeTheme(string current)
        {
            if (current.Equals("Dark"))
            {
                IsDark = false;
                ResourceLocator.SetColorScheme(Application.Current.Resources, IsDark ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);

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
