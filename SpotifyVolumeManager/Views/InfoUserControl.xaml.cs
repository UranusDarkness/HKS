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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;

namespace SpotifyVolumeManager.Views
{
    /// <summary>
    /// Logica di interazione per InfoUserControl.xaml
    /// </summary>
    public partial class InfoUserControl : UserControl
    {
        SpotifyClient spotify;
        public InfoUserControl()
        {
            InitializeComponent();
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }

        private void infoUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            versionTextBlock.Text += FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString()).FileVersion.ToString();
            if(Properties.Settings.Default.authToken != "")
            {
                spotify = new SpotifyClient(Properties.Settings.Default.authToken);
                try
                {
                    if (spotify.UserProfile.Current().Result.DisplayName != null)
                        changeColorButton.IsChecked = true;
                }
                catch (Exception)
                {

                    throw;
                }
                
            }
            
        }

        private void loginStatusCircle_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }
    }
}
