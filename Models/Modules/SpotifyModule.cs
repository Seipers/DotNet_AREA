using System;
using System.Collections.Generic;
using System.Linq;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace Area.Models
{
    class SpotifyModule : IModuleModel
    {
        SpotifyWebAPI spotify;
        int index;
        public string access_token;
        private Dictionary<int, Action<string>> reactions = new Dictionary<int, Action<string>>();
        private Dictionary<int, Func<string>> actions = new Dictionary<int, Func<string>>();
        public SpotifyModule(string access_token, int index)
        {
            reactions.Add(0, new Action<string>(PlayMusic));
            reactions.Add(1, new Action<string>(CreatePlaylist));
            reactions.Add(2, new Action<string>(AddMusicPlaylist));
            reactions.Add(3, new Action<string>(PlayAlbum));
            reactions.Add(4, new Action<string>(PlayArtist));
            reactions.Add(5, new Action<string>(PlayPlaylist));
            actions.Add(0, new Func<string>(getCurrentMusic));
            spotify = new SpotifyWebAPI()
            {
                TokenType = "Bearer",
                AccessToken = access_token
            };
            this.index = index;
            this.access_token = access_token;
        }

        public void PlayMusic(string name)
        {
            try
            {
                var musics = spotify.SearchItems(name, SearchType.All);
                 string track = musics.Tracks.Items.First().Uri;
                spotify.ResumePlayback("", "", new List<string> { track });
            }
            catch
            {

            }
        }
        public void PlayAlbum(string name)
        {
            
            try
            {
                var musics = spotify.SearchItems(name, SearchType.All);
                string album = musics.Albums.Items.First().Uri;
                spotify.ResumePlayback("", album);
            }
            catch{

            }

        }

        public void PlayPlaylist(string name)
        {
           
            try
            {
                var musics = spotify.SearchItems(name, SearchType.All);
                string playlist = musics.Playlists.Items.First().Uri;
                spotify.ResumePlayback("", playlist);
            }catch{

            }
        }

        public void PlayArtist(string name)
        {
            try
            {
                var musics = spotify.SearchItems(name, SearchType.All);
                string artist = musics.Artists.Items.First().Uri;
                spotify.ResumePlayback("", artist);
            }
            catch
            {
                
            }
        }
        public void CreatePlaylist(string name)
        {
            spotify.CreatePlaylist(spotify.GetPrivateProfile().Id, name);
        }

        public void AddMusicPlaylist(string music)
        {
            string playlist = "AreaPlaylist";
            var musics = spotify.SearchItems(music, SearchType.All);
            string track = "";
            foreach (var item in musics.Tracks.Items)
            {
                track = item.Uri;
                break;
            }
            string playlistid = null;
            while (playlistid == null)
            {
                var playlists = spotify.GetUserPlaylists(spotify.GetPrivateProfile().Id);
                foreach (var item in playlists.Items)
                {
                    if (item.Name == playlist)
                    {
                        playlistid = item.Id;
                        break;
                    }
                }
                if (playlistid == null)
                    CreatePlaylist(playlist);
            }
            if (playlistid != null)
                spotify.AddPlaylistTrack(spotify.GetPrivateProfile().Id, playlistid, track);
        }

        public string getCurrentMusic()
        {
            PlaybackContext track;
            try
            {
                track = spotify.GetPlayingTrack();   
                return (track.Item.Artists[0].Name + " - " + track.Item.Name);
            }
            catch (System.Exception)
            {
                return (null);                
            }
        }

        public string getAction()
        {
            return actions[index]();
        }

        public void postReaction(string reaction)
        {
            reactions[index](reaction);
        }
    }
}