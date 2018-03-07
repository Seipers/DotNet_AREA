using System.Linq;
using Area.DAT;

namespace Area.Models
{
    class SpotifyTwitterArea : IArea
    {
        SpotifyModule spotify;
        TwitterModule twitter;
        string spotifyToken = "";
        string twitterToken = "";
        string twitterTokenSecret = "";
        string last_event;
        string username;
        AREA area;

        public SpotifyTwitterArea(AREA area, AreaDbContext DB)
        {
            this.area = area;
            this.username = area.username;
            this.last_event = area.last_event;
            foreach (var token in DB.tokens)
            {
                if (token.type == "Spotify" && token.username == area.username)
                    spotifyToken = token.value;
                if (token.type == "TwitterAccessToken" && token.username == area.username)
                    twitterToken = token.value;
                if (token.type == "TwitterAccessTokenSecret" && token.username == area.username)
                    twitterTokenSecret = token.value;
            }
            spotify = new SpotifyModule(spotifyToken, area.index_action);
            twitter = new TwitterModule(twitterToken, twitterTokenSecret, area.index_reaction);
        }

        public bool isAvailable()
        {
            return (!string.IsNullOrEmpty(spotify.access_token) && !string.IsNullOrEmpty(twitter.access_token) &&
                    !string.IsNullOrEmpty(twitter.secret_token));
        }

        public void run(AreaDbContext DB)
        {
            string music = spotify.getAction();
            if (music == null || music == last_event)
                return;
            last_event = music;
            area.last_event = music;
            DB.areas.Update(area);
            DB.SaveChanges();
            twitter.postReaction("Currently play: " + music);
        }
    }
}