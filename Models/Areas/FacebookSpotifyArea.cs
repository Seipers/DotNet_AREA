using System;
using Area.DAT;

namespace Area.Models
{
    class FacebookSpotifyArea : IArea
    {
        SpotifyModule spotify;
        FacebookModule facebook;
        string spotifyToken = "";
        string facebookToken = "";
        string last_event;
        string username;
        AREA area;

        public FacebookSpotifyArea(AREA area, AreaDbContext DB)
        {
            this.area = area;
            this.username = area.username;
            this.last_event = area.last_event;
            foreach (var token in DB.tokens)
            {
                if (token.type == "Spotify" && token.username == area.username)
                    spotifyToken = token.value;
                if (token.type == "Facebook" && token.username == area.username)
                    facebookToken = token.value;
            }
            spotify = new SpotifyModule(spotifyToken, area.index_reaction);
            facebook = new FacebookModule(facebookToken, area.index_action);
        }

        public bool isAvailable()
        {
            return (!string.IsNullOrEmpty(spotify.access_token) && !string.IsNullOrEmpty(facebook.getAccessToken()));
        }

        public void run(AreaDbContext DB)
        {
            string info = facebook.getAction();
            if (info == null || info == last_event)
                return;
            last_event = info;
            area.last_event = info;
            DB.areas.Update(area);
            DB.SaveChanges();
            spotify.postReaction(info);
        }
    }
}