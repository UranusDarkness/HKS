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
using AdonisUI;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Configuration;

namespace SpotifyVolumeManager.Views
{
    /// <summary>
    /// Logica di interazione per HotKeyUserControl.xaml
    /// </summary>
    public partial class HotKeyUserControl : UserControl
    {
        IDictionary<string, TextBox> keyValuePairs;
        public HotKeyUserControl()
        {
            InitializeComponent();
            keyValuePairs = new Dictionary<string, TextBox>();
            keyValuePairs.Add("VolumeUp", volumeUpTextBox);
            keyValuePairs.Add("VolumeDown", volumeDownTextBox);
            keyValuePairs.Add("PausePlay", pausePlayTextBox);
            keyValuePairs.Add("NextTrack", nextTrackTextBox);
            keyValuePairs.Add("PreviousTrack", previousTrackTextBox);
        }

        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            
            Button btn = (Button)sender;
            string temp = btn.Name;

            temp = temp.Substring(5, temp.IndexOf("Button") - 5);
            TextBox tb = null;
            keyValuePairs.TryGetValue(temp, out tb);
            tb.Text = "";

        }

        private void saveHotKeysButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.volumeUp = volumeUpTextBox.Text;
            Properties.Settings.Default.volumeDown = volumeDownTextBox.Text;
            Properties.Settings.Default.pausePlay = pausePlayTextBox.Text;
            Properties.Settings.Default.previousTrack = previousTrackTextBox.Text;
            Properties.Settings.Default.nextTrack = nextTrackTextBox.Text;
            Properties.Settings.Default.Save();
            ConfigurationManager.RefreshSection("userSettings");
        }

        private void hotKeyUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (volumeUpTextBox.Text.Length > 0)
                volumeUpSwitch.IsChecked = true;
            if (volumeDownTextBox.Text.Length > 0)
                volumeDownSwitch.IsChecked = true;
            if (pausePlayTextBox.Text.Length > 0)
                pausePlaySwitch.IsChecked = true;
            if (nextTrackTextBox.Text.Length > 0)
                nextTrackSwitch.IsChecked = true;
            if (previousTrackTextBox.Text.Length > 0)
                previousTrackSwitch.IsChecked = true;
        }

        private void Switch_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            string temp = btn.Name;
            string temp1;

            temp1 = temp.Substring(1, temp.IndexOf("Switch")-1);
            temp = Char.ToUpper(temp[0]) + temp1;

            TextBox tb = null;
            keyValuePairs.TryGetValue(temp, out tb);
            tb.Text = "";
        }

        private void readHotKeyCombo(KeyEventArgs e, TextBox tb)
        {
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            // Build the shortcut key name.
            StringBuilder shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());

            // Update the text box.
            tb.Text = shortcutText.ToString();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            readHotKeyCombo(e, (TextBox)sender);
        }
    }
}
