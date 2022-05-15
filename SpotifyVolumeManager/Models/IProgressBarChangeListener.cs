using System;
using System.Collections.Generic;
using System.Text;

namespace SpotifyVolumeManager.Models
{
    public interface IProgressBarChangeListener
    {
        public void ProgressBarChange(int progress);
        public void DownloadCompleted();
    }
}
