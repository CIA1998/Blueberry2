using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace BlueberryPie
{
    /// <summary>
    ///     This is the media player class responsible for playing,pausing and resuming the specified audio
    /// </summary>
    public class Player
    {
        List<Song> songQueue = new List<Song>();
        public MediaElement mediaElement = new MediaElement();
        public Song currentSong = null;
        private Frame rootFrame;
        private MainPage page;

        /// <summary>
        ///     Set up the UI
        /// </summary>
        public Player()
        {
            rootFrame = Window.Current.Content as Frame;
            page = rootFrame.Content as MainPage;
        }

        /// <summary>
        ///     Attamts to call a command line app which can download youtube videos.
        /// </summary>
        /// <param name="videoURL">The  URL of the video which you wish to download with the youtube-dl application</param>
        public void DownloadVideo(string videoURL)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            //string command = "cd %USERPROFILE%\\Music & youtube-dl --extract-audio --audio-format mp3 " + videoURL;
            string command = "/k echo 123";
            p.StartInfo.FileName = "'" + Directory.GetCurrentDirectory() + "\\run.bat" + '"';
            p.StartInfo.Arguments = command;
            p.StartInfo.Verb = "runas";
            p.Start();

            Debug.WriteLine(command);
        }

        /// <summary>
        ///     Pause the mediaElement
        /// </summary>
        public void PauseSong()
        {
            this.mediaElement.Pause();
        }

        /// <summary>
        ///     Start playing specified song t=with the MediaElement
        /// </summary>
        /// <param name="song"> Song object which contains the location of the song file</param>
        public async Task StartSongAsync(Song song)
        {
            this.currentSong = song;
            String songName = Path.GetFileName(song.songPath);
            var storageFile = await KnownFolders.MusicLibrary.GetFileAsync(songName);
            var stream = await storageFile.OpenAsync(FileAccessMode.Read);
            this.mediaElement.SetSource(stream, storageFile.ContentType);
            this.mediaElement.Play();
            page.SetPathName(song);
            Debug.WriteLine(this.mediaElement.CurrentState);
        }

        /// <summary>
        ///     Resume the mediaElement
        /// </summary>
        public void ResumeSong()
        {
            this.mediaElement.Play();

        }

        /// <summary>
        ///     This will play all the songs in the queue one after another. Set the starting song to
        ///     be the first song in the SongQueue. The timer located in the MainPage will change the songs
        /// </summary>
        public void PlayAllSongs()
        {
            StartSongAsync(songQueue[0]);
        }

        /// <summary>
        ///     Return the songQueue containing all the song objects
        /// </summary>
        /// <returns></returns>
        public List<Song> GetSongs()
        {
            return songQueue;
        }

        /// <summary>
        ///     Remove a song from the songQueue at specified index
        /// </summary>
        /// <param name="index">Index at which you wish to remove the song</param>
        public void RemoveSong(int index)
        {
            //Dequeue
            songQueue.RemoveAt(index);
        }

        /// <summary>
        ///     Add a specific song to the song queue
        /// </summary>
        /// <param name="song">Song that you wish to add</param>
        public void AddSongToQueue(Song song)
        {
            songQueue.Add(song);
        }
    }
}
