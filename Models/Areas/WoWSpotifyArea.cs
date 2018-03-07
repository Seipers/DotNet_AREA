using System;
using System.Collections.Generic;
using Area.Models;
using Area.DAT;
using System.Linq;

namespace Area.Models
{
    public class WoWSpotifyArea : IArea
    {
        private WoWModuleModel WoW;
        private SpotifyModule Spotify;
        private AREA area;


        public WoWSpotifyArea(AREA area, AreaDbContext DB)
        {
            this.area = area;
            string wow_access_token = "";
            string spotify_access_token = "";
            foreach (var token in DB.tokens)
            {
                if (token.type == "WoW" && token.username == area.username)
                    wow_access_token = token.value;
                if (token.type == "Spotify" && token.username == area.username)
                    spotify_access_token = token.value;
            }
            WoW = new WoWModuleModel(wow_access_token, area.index_action);
            Spotify = new SpotifyModule(spotify_access_token, area.index_reaction);
        }

        public void  run(AreaDbContext DB)
        {
            List<WoWCharacterModel> characters = DB.wowcharactermodels.ToList();            
            if (WoW.getIndex() == 0)
            {
                List<WoWCharacterModel> new_characters = WoW.getCharacters();
                foreach (var character in new_characters)
                {
                    if (!WoW.alreadyExists(characters, character))
                    {
                        DB.wowcharactermodels.Add(character);
                        DB.SaveChanges();
                        Spotify.postReaction(WoW.getAction());
                    }
                }
            }
            else
            {
                foreach (var character in characters)
                {
                    if (WoW.checkAndUpdateCharacter(character, DB))
                    {
                        Spotify.postReaction(WoW.getAction());
                    }
                }
            }
        }

        public bool isAvailable()
        {
            return (!String.IsNullOrEmpty(WoW.getAccessToken()) && !String.IsNullOrEmpty(Spotify.access_token));
        }
    }
}