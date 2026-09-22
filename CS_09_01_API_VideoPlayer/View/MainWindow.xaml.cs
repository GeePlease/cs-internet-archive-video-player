using CS_09_01_API_VideoPlayer.ViewModel;
using LibVLCSharp.Shared;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CS_09_01_API_VideoPlayer.View
{
    public partial class MainWindow : Window
    {
        // ATTRIBUTES
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;
        private bool _isUpdatingPosition; // for video position slider


        // CONSTRUCTOR
        public MainWindow()
        {
            InitializeComponent();

            // initialize VLC
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);

            // connect media player with VideoView
            videoView.MediaPlayer = _mediaPlayer;

            // react to changes in ViewModel
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
        }


        // react to changed video source
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.MediaPlayerSource))
            {
                PlaySelectedVideo();
            }
        }


        // play selected video
        private void PlaySelectedVideo()
        {
            // get ViewModel
            if (DataContext is not MainWindowViewModel viewModel)
                return;

            // check if video URL exists
            if (viewModel.MediaPlayerSource == null)
                return;

            // create VLC media from Internet Archive URL
            using var media = new Media(
                _libVLC,
                viewModel.MediaPlayerSource);

            // play video
            _mediaPlayer.Play(media);
        }


        // change text box
        private void TextBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
        }


        // play button click
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            PlaySelectedVideo();
        }


        // pause button click
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Pause();
        }


        // volume slider
        private void VolumeSlider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Volume = (int)e.NewValue;
            }
        }

        // video position slider
        private void PositionSlider_ValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (_mediaPlayer == null || _isUpdatingPosition)
                return;

            _mediaPlayer.Position = (float)(e.NewValue / 100);
        }
    }
}