using System;
using System.Linq;
using Area.DAT;

namespace Area.Models
{
    class FacebookTwitterArea : IArea
    {
        FacebookModule facebook;
        TwitterModule twitter;
        string FacebookToken = "";
        string twitterToken = "";
        string twitterTokenSecret = "";
        string last_event;
        string username;
        AREA area;

        public FacebookTwitterArea(AREA area, AreaDbContext DB)
        {
            this.area = area;
            this.username = area.username;
            this.last_event = area.last_event;
            foreach (var token in DB.tokens)
            {
                if (token.type == "Facebook" && token.username == area.username)
                    FacebookToken = token.value;
                if (token.type == "TwitterAccessToken" && token.username == area.username)
                    twitterToken = token.value;
                if (token.type == "TwitterAccessTokenSecret" && token.username == area.username)
                    twitterTokenSecret = token.value;
            }
            facebook = new FacebookModule(FacebookToken, area.index_action);
            twitter = new TwitterModule(twitterToken, twitterTokenSecret, area.index_reaction);
        }

        public bool isAvailable()
        {
            return (!string.IsNullOrEmpty(facebook.getAccessToken()) && !string.IsNullOrEmpty(twitter.access_token) &&
                    !string.IsNullOrEmpty(twitter.secret_token));
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
            twitter.postReaction(info);
        }
    }
}