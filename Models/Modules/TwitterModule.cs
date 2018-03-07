using System;
using System.Collections.Generic;
using System.Linq;
using TweetSharp;

namespace Area.Models
{
    class TwitterModule : IModuleModel
    {
        private int index;
        TwitterService twitter;
        public string access_token;
        public string secret_token;
        private Dictionary<int, Action<string>> reactions = new Dictionary<int, Action<string>>();
        private Dictionary<int, Func<string>> actions = new Dictionary<int, Func<string>>();

        public TwitterModule(string AccessToken, string AccessTokenSecret, int index)
        {
            reactions.Add(0, new Action<string>(PostTweet));
            actions.Add(0, new Func<string>(GetMyLastTweet));
            actions.Add(1, new Func<string>(GetLastHashTag));
            twitter = new TwitterService("CI6yhnDqdfVvm8TVckYlIfadl", "MvCtuuS9nPLHPLx7yRsrM8x7DvArtW46pPgfn4okXwfL5UThL5", AccessToken, AccessTokenSecret);
            this.index = index;
            this.access_token = AccessToken;
            this.secret_token = AccessTokenSecret;
        }

        public void PostTweet(string post)
        {
            try
            {
                twitter.SendTweet(new SendTweetOptions {Status = post});                
            }
            catch (System.Exception)
            {
            }
        }

        public string GetMyLastTweet()
        {
            var list = twitter.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions());
            var tweet = list.First();
            return (tweet.Text);
        }

        public string GetLastHashTag()
        {
            try{
                var list = twitter.Search(new SearchOptions { Q = "#AreaMusic", SinceId=29999 }).Statuses;
                var text = list.First().Text;
                return (text.Replace("#AreaMusic", ""));
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