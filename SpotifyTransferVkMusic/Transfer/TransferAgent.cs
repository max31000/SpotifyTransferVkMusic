using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SpotifyTransferVkMusic
{
    public class TransferAgent
    {
        public SpotifyAgent SpotifyAgent { get; private set; } = new SpotifyAgent();
        public VkAgent VkAgent { get; private set; } = new VkAgent();

        public delegate void AddSong(Song song);
        public event AddSong OnAddSongEvent;
        public delegate void End();
        public event End OnEndFindSongEvent;
        public event End OnEndAddSongsEvent;

        public async Task SpotifyAuth() => await SpotifyAgent.Auth();
        public async Task VkAuth() => await VkAgent.Auth();

        public async void TransferMusic(string playlistName, IList<string> spotifyTrackUris)
        {
            if (!SpotifyAgent.IsAuth)
                throw new AuthenticationException();

            var userSpotifyPlaylist = await SpotifyAgent.GetOrCreatePlaylist(playlistName);

            var maxAddSongsInRequestCount = 99;

            for (var i = 0; maxAddSongsInRequestCount * i < spotifyTrackUris.Count; i++)
            {
                var tracksRange = new List<string>();

                for (var j = 0; i * maxAddSongsInRequestCount + j < spotifyTrackUris.Count && j < maxAddSongsInRequestCount; j++)
                    tracksRange.Add(spotifyTrackUris[i * maxAddSongsInRequestCount + j]);

                await SpotifyAgent.AddSongsToPlaylist(tracksRange, userSpotifyPlaylist);
            }

            OnEndAddSongsEvent?.Invoke();
        }

        public async void GenerateSpotifyTracksUris(string vkPageUrl)
        {
            var vkTracks = VkAgent.GetMusicList(vkPageUrl);

            foreach (var vkTrack in vkTracks)
            {
                var spotifyTrack = await SpotifyAgent.FindSong(vkTrack.Artist, vkTrack.Title);

                if (spotifyTrack is null)
                    continue;

                var song = new Song(
                        String.Join(" ", spotifyTrack.Artists.Select(a => a.Name)),
                        spotifyTrack.Name,
                        spotifyTrack.Uri);

                OnAddSongEvent?.Invoke(song);
            }

            OnEndFindSongEvent?.Invoke();
        }
    }
}
