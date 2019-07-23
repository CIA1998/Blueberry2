using System;
using System.ComponentModel;

namespace Blueberry2
{
    public class Song : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _songArtist;
        private string _songName;
        private string _songLenght;
        private string _songPath;

        /// <summary>
        ///     get or set the songArtist for a song
        /// </summary>
        public String songArtist
        {
            get
            {
                return _songArtist;
            }

            set
            {
                _songArtist = value;
                OnPropertyChanged("songArtist");
            }
        }

        /// <summary>
        ///     get or set the songName for a song object
        /// </summary>
        public String songName
        {
            get
            {
                return _songName;
            }

            set
            {
                _songName = value;
                OnPropertyChanged("songName");
            }
        }

        /// <summary>
        ///     get or set a songLenght for a song object
        /// </summary>
        public String songLenght
        {
            get
            {
                return _songLenght;
            }

            set
            {
                _songLenght = value;
                OnPropertyChanged("songLenght");
            }
        }

        /// <summary>
        ///     get or set the song path for a Song object
        /// </summary>
        public String songPath
        {
            get
            {
                return this._songPath;
            }
            set
            {
                this._songPath = value;
            }
        }

        public Song()
        {
        }

        /// <summary>
        ///     Refresh data grid when song name changed event
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
