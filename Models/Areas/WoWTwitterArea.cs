using System;
using System.Collections.Generic;
using System.Linq;
using Area.DAT;
using Area.Models;

namespace Area.Models
{
    public class WoWTwitterArea : IArea
    {
        private WoWModuleModel WoW;
        private TwitterModule Twitter;
        private AREA area;

        public WoWTwitterArea(AREA area, AreaDbContext DB)
        {
            this.area = area;
            string wow_access_token = "";
            string twitter_access_token = "";
            string twitter_secret_token = "";
            foreach (var token in DB.tokens)
            {
                if (token.type == "WoW" && token.username == area.username)
                    wow_access_token = token.value;
                if (token.type == "TwitterAccessToken" && token.username == area.username)
                    twitter_access_token = token.value;
                if (token.type == "TwitterAccessTokenSecret" && token.username == area.username)
                    twitter_secret_token = token.value;
            }
            WoW = new WoWModuleModel(wow_access_token, area.index_action);
            Twitter = new TwitterModule(twitter_access_token, twitter_secret_token, area.index_reaction);
        }

        public void  run(AreaDbContext DB)
        {
            List<WoWCharacterModel> characters = DB.wowcharactermodels.ToList();            
            if (WoW.getIndex() == 6)
            {
                List<WoWCharacterModel> new_characters = WoW.getCharacters();
                foreach (var character in new_characters)
                {
                    if (!WoW.alreadyExists(characters, character))
                    {
                        DB.wowcharactermodels.Add(character);
                        DB.SaveChanges();
                        Twitter.postReaction(WoW.getCharacterAction(character));
                    }
                }
            }
            else
            {
                foreach (var character in characters)
                {
                    if (WoW.checkAndUpdateCharacter(character, DB))
                    {
                        Twitter.postReaction(WoW.getCharacterAction(character));
                    }

                }
            }
        }

        public bool isAvailable()
        {
            return (!String.IsNullOrEmpty(WoW.getAccessToken()) && !String.IsNullOrEmpty(Twitter.access_token) &&
            !String.IsNullOrEmpty(Twitter.access_token));
        }
    }
}