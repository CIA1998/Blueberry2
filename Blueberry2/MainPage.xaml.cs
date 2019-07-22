using BlueberryPie.statics;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Diagnostics;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net;
using System.Linq;

namespace BlueberryPie
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Player player = new Player();
        DispatcherTimer Timer1 = new DispatcherTimer();

        /// <summary>
        ///     This is the MainPage which will contain the sonqQueue elements og which can be played
        /// </summary>
        public MainPage()
        {
            Debug.WriteLine("Constructor of MainPage");
            this.InitializeComponent();
            //Enable caching on main page,so that the songs remain n the page when user goes to different screen
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            PageCore.RequestedTheme = GlobalSettings.appTheme;
            this.DataContext = player.GetSongs();
            this.DownloadFromLocalhost();
            this.InitTimer();
        }

        public Player GetPlayer()
        {
            return player;
        }

        /// <summary>
        ///     Remove the selected song from the MainScreen and in turn the songQueue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveSongFromQueue(object sender, RoutedEventArgs e)
        {
            //Return the selected row.
            DataGridRow row = DataGridRow.GetRowContainingElement((DataGridCell)((Button)sender).Parent);
            player.RemoveSong(row.GetIndex());
            UpdateQueueList();
        }

        /// <summary>
        ///     Play the selected song by calling the startSongAsync() in the PlayerClass.
        ///     Additionally call teh SetPathName() to display the current song playing and set the visibility of the resume and pause button. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PlaySongFromQueue(object sender, RoutedEventArgs e)
        {
            DataGridRow row = DataGridRow.GetRowContainingElement((DataGridCell)((Button)sender).Parent);
            Song chosenSong = (Song)row.DataContext;
            player.StartSongAsync(chosenSong);
            SetPathName(chosenSong);
            resumeButton.Visibility = Visibility.Collapsed;
            pauseButton.Visibility = Visibility.Visible;
            UpdateQueueList();
            Timer1.Stop();
        }

        /// <summary>
        ///     Setting the name of the current song that is playing
        /// </summary>
        /// <param name="pathText">Field in the xaml file</param>
        public void SetPathName(Song song)
        {
            pathText.Text = "Now playing: " + song.songName;
        }

        /// <summary>
        ///     Setting the visibility of 'Play All' button
        /// </summary>
        /// <param name="playAllButton">Field in the xaml file</param>
        public void MakePlayAllVisible()
        {
            playAllButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Create a timer which calls the Tick every 500ms
        /// </summary>
        public void InitTimer()
        {
            Timer1.Interval = TimeSpan.FromMilliseconds(500);
            Timer1.Tick += Timer1_Tick;
            Timer1.Start();
        }

        /// <summary>
        ///     This function continuously checks for the state of the media layer, to see if it 
        ///     has stopped playing a song. If so, then the next song in the queue should be played.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1_Tick(object sender, object e)
        {

            if (this.player.mediaElement.CurrentState.ToString() == "Paused" &&
                this.player.mediaElement.Position.Seconds == this.player.mediaElement.NaturalDuration.TimeSpan.Seconds)
            {

                //Find the index of the song being played in the songQueue
                int index = player.GetSongs().FindIndex(a => a.songName == player.currentSong.songName);
                if (index + 1 > player.GetSongs().Count - 1)
                {
                    Debug.WriteLine("reset");
                    index = -1;
                }
                player.StartSongAsync(player.GetSongs()[index + 1]);
                SetPathName(player.GetSongs()[index + 1]);
            }
        }

        /// <summary>
        ///     Copying files from removable storage to music folder of the host device
        /// </summary>
        /// <param name="targetFolderName"></param>
        public async void USBDriveCopyFolder()
        {
            var targetFolderName = "music";

            var removableDevice = (await KnownFolders.RemovableDevices.GetFoldersAsync()).FirstOrDefault();
            if (null == removableDevice)
            {
                //Debug.WriteLine("removableDevice is null !");
                return;
            }
            Debug.WriteLine(removableDevice.Name + ":\n");

            var sourceFolder = await removableDevice.GetFolderAsync(targetFolderName);
            if (null == sourceFolder)
            {
                //Debug.WriteLine(targetFolderName + " folder is not found !");
                return;
            }
            //Debug.WriteLine(sourceFolder.Name + ":\n");

            var destFodler = KnownFolders.MusicLibrary;
            if (null == destFodler)
            {
                //Debug.WriteLine("KnownFolders.DocumentsLibrary folder get failed !");
                return;
            }

            var files = await sourceFolder.GetFilesAsync();

            foreach (var file in files)
            {
                //Debug.WriteLine(file.Name + "\n");
                await file.CopyAsync(destFodler);
            }

        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name=""></param>
        public void PlayAllSongs(object sender, RoutedEventArgs e)
        {
            player.PlayAllSongs();
            pauseButton.Visibility = Visibility.Visible;
            playAllButton.Visibility = Visibility.Collapsed;
            SetPathName(player.currentSong);
            Timer1.Start();
        }

        /// <summary>
        ///     Resuming song
        /// </summary>
        /// <param name="resumeButton"></param>
        /// <param name="pauseButton"></param>
        public void ResumeSongFromQueue(object sender, RoutedEventArgs e)
        {
            this.player.ResumeSong();
            resumeButton.Visibility = Visibility.Collapsed;
            pauseButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     Pausing song, that is in queue
        /// </summary>
        /// <param name="resumeButton"></param>
        /// <param name="pauseButton"></param>
        public void PauseSongFromQueue(object sender, RoutedEventArgs e)
        {
            pauseButton.Visibility = Visibility.Collapsed;
            resumeButton.Visibility = Visibility.Visible;
            resumeButton.Content = "Resume Song";
            player.PauseSong();
            UpdateQueueList();
        }

        /// <summary>
        ///     Refreshing the list of the songs in the grid (updating list)
        /// </summary>
        /// <param name="queueGrid">Grid in xaml</param>
        public void UpdateQueueList()
        {
            queueGrid.ItemsSource = null;
            queueGrid.ItemsSource = player.GetSongs();
        }

        /// <summary>
        ///     Opens side menu
        /// </summary>
        /// <param name="sideMenu"></param>
        private void ShowMenu(object sender, RoutedEventArgs e)
        {
            sideMenu.IsPaneOpen = !sideMenu.IsPaneOpen;
        }

        /// <summary>
        ///     Navigates to Songs page
        /// </summary>
        private void OpenSongsPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UIChooseSong));
        }

        /// <summary>
        ///     Navigates to Settings page
        /// </summary>
        private void OpenSettingsPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }

        /// <summary>
        ///     Downloads songs from local storage
        /// </summary>
        /// <param name="localIp">IP of the local machine</param>
        async void DownloadFromLocalhost()
        {

            var i = 1;
            // Whatever the localhost ip is
            var localIp = "http://192.168.178.18:8888";
            string audioURL = localIp + "/music/" + i + ".mp3";

            while (this.URLExists(audioURL))
            {

                var myFilter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
                myFilter.AllowUI = false;
                Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient(myFilter);
                Windows.Web.Http.HttpResponseMessage result = await client.GetAsync(new Uri(audioURL));

                // Saving a file with the number
                var file = await KnownFolders.MusicLibrary.CreateFileAsync(i + " song.mp3", CreationCollisionOption.ReplaceExisting);

                using (var filestream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await result.Content.WriteToStreamAsync(filestream);
                    await filestream.FlushAsync();

                }

                i++;
                audioURL = localIp + "/music/" + i + ".mp3";  //"http://docs.google.com/uc?export=download&id=0B6u8YFm5Y0GeLXRtd2Z6X0RjNTA";
            }
        }

        /// <summary>
        ///     Checks whether the URL is reachable
        /// </summary>
        public bool URLExists(string url)
        {
            bool result = true;

            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Timeout = 1200; // miliseconds
            webRequest.Method = "HEAD";

            try
            {
                webRequest.GetResponse();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private void LogException(string v, Exception ex)
        {
            throw new NotImplementedException();
        }

        private void HandleDownloadAsync(DownloadOperation download, bool v)
        {
            throw new NotImplementedException();
        }
    }
}
