using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace SpotifyTransferVkMusic
{
    public class SpotifyAgent
    {
        public bool IsAuth => Client != null;
        public delegate void OnAuth();
        public event OnAuth OnAuthEvent;

        private SpotifyClient Client { get; set; }

        private static EmbedIOAuthServer _server;
        

        public async Task Auth()
        {
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5000/callback"), 5000);
            await _server.Start().ConfigureAwait(false);

            _server.ImplictGrantReceived += OnImplicitGrantReceived;

            var request = new LoginRequest(_server.BaseUri, "46f6cbff0b5e4bd1845f35f7c661c6b1", LoginRequest.ResponseType.Token)
            {
                Scope = new List<string> { Scopes.UserLibraryRead, Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic }
            };
            BrowserUtil.Open(request.ToUri());
        }

        public async Task<FullTrack> FindSong(string Author, string Name)
        {
            if (!IsAuth)
                throw new AuthenticationException();

            var searchRequest = new SearchRequest(SearchRequest.Types.Track, Author + " " + Name);
            var searchAnswer = await Client.Search.Item(searchRequest);

            return searchAnswer.Tracks.Items.Count != 0 
                ? searchAnswer.Tracks.Items[0] 
                : null;
        }

        public async Task AddSongsToPlaylist(IList<string> tracksUris, SimplePlaylist playlist)
        {
            var songAddRequest = new PlaylistAddItemsRequest(tracksUris);

            await Client.Playlists.AddItems(playlist.Id, songAddRequest);
        }

        public async Task<SimplePlaylist> CreatePlaylist(string name)
        {
            var client = await Client.UserProfile.Current();
            var playlistReq = new PlaylistCreateRequest(name);
            var playlist = await Client.Playlists.Create(client.Id, playlistReq);
            var playlists = await GetPlaylists();

            return playlists
                    .Where(p => p.Id.Equals(playlist.Id))
                    .First();
        }

        public async Task<List<SimplePlaylist>> GetPlaylists()
        {
            var playlists = await Client.Playlists.CurrentUsers();

            return playlists.Items;
        }

        public async Task<SimplePlaylist> GetOrCreatePlaylist(string name)
        {
            var playlists = await GetPlaylists();
            SimplePlaylist findedPlaylist = null;

            foreach (var playlist in playlists)
            {
                if (playlist.Name.Equals(name))
                {
                    findedPlaylist = playlist;
                    break;
                }
            }

            if (findedPlaylist is null)
                findedPlaylist = await CreatePlaylist(name);

            return findedPlaylist;
        }

        private async Task OnImplicitGrantReceived(object sender, ImplictGrantResponse response)
        {
            await _server.Stop();
            var spotify = new SpotifyClient(response.AccessToken);
            Client = spotify;
            OnAuthEvent?.Invoke();
        }
    }
}
