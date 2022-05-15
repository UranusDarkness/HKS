﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Reflection;
using Forms = System.Windows.Forms;
using SpotifyAPI.Web;
using System.ComponentModel;
using SpotifyAPI.Web.Auth;
using AutoUpdaterDotNET;
using System.Windows.Controls.Primitives;

namespace SpotifyVolumeManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static SpotifyClient spotify;
        private readonly Forms.NotifyIcon _notifyIcon;
        //private readonly string OAuth_Token = "";
        public int volume_up_id;
        public int volume_down_id;
        string triggerKey;
        string modKey;
        TypeConverter converterKeys = TypeDescriptor.GetConverter(typeof(Forms.Keys));
        TypeConverter converterMods = TypeDescriptor.GetConverter(typeof(KeyModifiers));
        private static EmbedIOAuthServer _server;

        public App()
        {
            _notifyIcon = new Forms.NotifyIcon();
            //spotify = new SpotifyClient(OAuth_Token);
            doStuff();   
        }
        public void doStuff()
        {
            /*System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings["volumeUp"].Value != "" && configuration.AppSettings.Settings["volumeDown"].Value != "")
            {
                triggerKey = "";
                modKey = "";
                int pluspos = 0;


                for (int i = 0; i < configuration.AppSettings.Settings["volumeUp"].Value.Length; i++)
                {
                    if (configuration.AppSettings.Settings["volumeUp"].Value[i] != '+')
                    {
                        modKey += configuration.AppSettings.Settings["volumeUp"].Value[i];
                    }
                    else
                    {
                        pluspos = i;
                        break;
                    }

                }

                if (pluspos > 0)
                {
                    for (int i = pluspos + 1; i < configuration.AppSettings.Settings["volumeUp"].Value.Length; i++)
                    {
                        triggerKey += configuration.AppSettings.Settings["volumeUp"].Value[i];
                    }
                }
                if(modKey != "")
                {
                    volume_up_id = HotKeyManager.RegisterHotKey((Forms.Keys)Enum.Parse(typeof(Forms.Keys), triggerKey), (KeyModifiers)Enum.Parse(typeof(KeyModifiers), modKey));
                }
                else
                {
                    volume_up_id = HotKeyManager.RegisterHotKey((Forms.Keys)Enum.Parse(typeof(Forms.Keys), triggerKey), 0);
                }


                pluspos = 0;
                modKey = "";
                triggerKey = "";
                for (int i = 0; i < configuration.AppSettings.Settings["volumeDown"].Value.Length; i++)
                {
                    if (configuration.AppSettings.Settings["volumeDown"].Value[i] != '+')
                    {
                        modKey += configuration.AppSettings.Settings["volumeDown"].Value[i];
                    }
                    else
                    {
                        pluspos = i;
                        break;
                    }

                }

                if (pluspos > 0)
                {
                    for (int i = pluspos + 1; i < configuration.AppSettings.Settings["volumeDown"].Value.Length; i++)
                    {
                        triggerKey += configuration.AppSettings.Settings["volumeDown"].Value[i];
                    }
                }

                if (modKey != "")
                {
                    volume_down_id = HotKeyManager.RegisterHotKey((Forms.Keys)Enum.Parse(typeof(Forms.Keys), triggerKey), (KeyModifiers)Enum.Parse(typeof(KeyModifiers), modKey));
                }
                else
                {
                    volume_down_id = HotKeyManager.RegisterHotKey((Forms.Keys)Enum.Parse(typeof(Forms.Keys), triggerKey), 0);
                }
               
                HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
            }*/
            
        }
        void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            triggerKey = "";
            modKey = "";
            int pluspos = 0;


            for (int i = 0; i < configuration.AppSettings.Settings["volumeUp"].Value.Length; i++)
            {
                if (configuration.AppSettings.Settings["volumeUp"].Value[i] != '+')
                {
                    modKey += configuration.AppSettings.Settings["volumeUp"].Value[i];
                }
                else
                {
                    pluspos = i;
                    break;
                }

            }

            if (pluspos > 0)
            {
                for (int i = pluspos + 1; i < configuration.AppSettings.Settings["volumeUp"].Value.Length; i++)
                {
                    triggerKey += configuration.AppSettings.Settings["volumeUp"].Value[i];
                }
            }

            if(e.Key == (Forms.Keys)Enum.Parse(typeof(Forms.Keys), triggerKey) && e.Modifiers == (KeyModifiers)Enum.Parse(typeof(KeyModifiers), modKey))
            {
                var volume = spotify.Player.GetCurrentPlayback().Result.Device.VolumePercent;
                int newvol = (int)volume;
                if(newvol <= 95)
                    newvol += 5;
                spotify.Player.SetVolume(new PlayerVolumeRequest(newvol));
            }
            else
            {
                var volume = spotify.Player.GetCurrentPlayback().Result.Device.VolumePercent;
                int newvol = (int)volume;
                if(newvol >= 5)
                    newvol -= 5;
                spotify.Player.SetVolume(new PlayerVolumeRequest(newvol));
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            Assembly myAss = Assembly.GetExecutingAssembly();
            Stream myStre = myAss.GetManifestResourceStream(myAss.GetName().Name + ".Resources.img.spotifyVolumeManagerLogo.ico");
            _notifyIcon.Icon = new System.Drawing.Icon(myStre);
            _notifyIcon.Text = "Spotify Volume Manager";

            _notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Login", null, OnLoginClicked);
            _notifyIcon.ContextMenuStrip.Items.Add("Settings", null, OnKeybindsClicked);
            _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, OnExitClicked);
            _notifyIcon.Visible = true;

            base.OnStartup(e);

            /*code behind to check if auto update is enabled
             * 
            System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if(configuration.AppSettings.Settings["autoUpdate"].Value == "True")
                MessageBox.Show(configuration.AppSettings.Settings["autoUpdate"].Value, "Enabled auto update", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show(configuration.AppSettings.Settings["autoUpdate"].Value, "Disabled auto update", MessageBoxButton.OK, MessageBoxImage.Warning);
            */
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await startServer();
        }


        public static async Task startServer()
        {
            // Make sure "http://localhost:5000/callback" is in your spotify application as redirect uri!
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
            await _server.Start();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, "343c40fdfafc46f082091816508179c6", LoginRequest.ResponseType.Code)
            {
                Scope = new List<string> { Scopes.UserReadEmail, Scopes.UserModifyPlaybackState, Scopes.UserReadPlaybackState }
            };
            BrowserUtil.Open(request.ToUri());
        }

        private static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
        {
            await _server.Stop();

            var config = SpotifyClientConfig.CreateDefault();
            var tokenResponse = await new OAuthClient(config).RequestToken(
              new AuthorizationCodeTokenRequest(
                "343c40fdfafc46f082091816508179c6", "3a11ec6a038d4003a077d30ec93efa2b", response.Code, new Uri("http://localhost:5000/callback")
              )
            );
            SpotifyVolumeManager.Properties.Settings.Default.authToken = tokenResponse.AccessToken;
            SpotifyVolumeManager.Properties.Settings.Default.Save();
            spotify = new SpotifyClient(tokenResponse.AccessToken);
            // do calls with Spotify and save token?
        }

        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }




        private void OnKeybindsClicked(object sender, EventArgs e)
        {
            /*KeyBindsSetter keyBindsSetter = new KeyBindsSetter(this, volume_down_id, volume_up_id);
            keyBindsSetter.Show();*/
            HotKeySet hotKeySet = new HotKeySet();
            hotKeySet.Show();
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            _notifyIcon.Dispose();
            Application.Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {

            _notifyIcon.Dispose();
            base.OnExit(e);
        }
       
    }
}
