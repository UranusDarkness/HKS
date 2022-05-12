using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SpotifyVolumeManager.Views
{
    /// <summary>
    /// Logica di interazione per UpdatesUserControl.xaml
    /// </summary>
    public partial class UpdatesUserControl : UserControl
    {
        public UpdatesUserControl()
        {
            InitializeComponent();
        }

        private void updatesUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            hksVersionUpdateTextBlock.Text += FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString()).FileVersion.ToString();
        }
    }
}
