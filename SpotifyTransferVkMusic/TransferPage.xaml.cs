using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyTransferVkMusic
{
    /// <summary>
    /// Логика взаимодействия для TransferPage.xaml
    /// </summary>
    public partial class TransferPage : Page
    {
        private TransferAgent transferAgent;
        private List<Song> spotifySongs = new List<Song>();

        public TransferPage(TransferAgent agent)
        {
            transferAgent = agent;
            InitializeComponent();
            var autoEvent = new AutoResetEvent(false);
            AddButton.Click += (o, e) => AddTracksButton();
            agent.OnAddSongEvent += (song) => Dispatcher.Invoke(() => AddSongToList(song));
            agent.OnEndFindSongEvent += () => Dispatcher.Invoke(UnblockAddButton);
            agent.OnEndAddSongsEvent += () => Dispatcher.Invoke(End);
            agent.GenerateSpotifyTracksUris(agent.VkAgent.GetUserUrl());
        }

        private void UnblockAddButton()
        {
            AddButton.IsEnabled = true;
        }

        private void End()
        {
            NavigationService.Navigate(new EndPage());
        }

        private void AddTracksButton()
        {
            transferAgent.TransferMusic(playlistNameTextbox.Text, spotifySongs.Select(s => s.Uri).ToList());
        }

        private void AddSongToList(Song song)
        {
            spotifySongs.Add(song);
            var checkbox = new CheckBox() { Content = song.Author + " - " + song.Name, IsChecked = true };
            checkbox.Unchecked += (o, e) => spotifySongs.Remove(song);
            checkbox.Checked += (o, e) => spotifySongs.Add(song);
            songsListBox.Items.Add(checkbox);
        }
    }
}
