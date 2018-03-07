using Area.DAT;
using Area.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Area.Models
{
    public class WoWModuleModel : IModuleModel
    {
        private string  access_token;
        private int     index;

        public WoWModuleModel(string token, int index)
        {
            this.access_token = token;
            this.index = index;
        }
        public void     setAccessToken(string token)
        {
            this.access_token = token;
        }

        public string   getAccessToken()
        {
            return (this.access_token);
        }

        public void     setIndex(int idx)
        {
            this.index = idx;
        }

        public int      getIndex()
        {
            return this.index;
        }

        public int   getDeaths(string realm, string name)
        {
            WoWCharacterModel character = getAllInfos(realm, name);
            return (character.deaths);
        }

        public int   getFacepalmed(string realm, string name)
        {
             WoWCharacterModel character = getAllInfos(realm, name);
            return (character.facepalmed);
        }

        public int   getLevel(string realm, string name)
        {
            try {
                var client = new WebClient();
                client.UseDefaultCredentials = true;         
                client.QueryString.Add("apikey", "ak8a3kw6ksu5y89t8bx5ct8f4kp5uzv5");
                client.QueryString.Add("fields", "statistics");
                client.QueryString.Add("locale", "en_GB");
                string result = client.DownloadString("https://eu.api.battle.net/wow/character/" + realm + "/" + name);
                dynamic obj = JsonConvert.DeserializeObject(result);
                return (obj["level"]);
            }
            catch
            {
                return (0);
            }
        }

        public int   getQuestsDone(string realm, string name)
        {
             WoWCharacterModel character = getAllInfos(realm, name);
            return (character.questsDone);
        }

        public int   getFishCaught(string realm, string name)
        {
            WoWCharacterModel character = getAllInfos(realm, name);
            return (character.fishCaught);
        }

        public List<WoWCharacterModel> getCharacters()
        {
            var client = new WebClient();
            client.UseDefaultCredentials = true;
            byte[] response = client.UploadValues("https://eu.api.battle.net/wow/user/characters", new NameValueCollection()
            {
                {"access_token", this.access_token}
            });
            string result = System.Text.Encoding.UTF8.GetString(response);
            WoWCharacterList characterList = JsonConvert.DeserializeObject<WoWCharacterList>(result);
            return characterList.characters;
        }

        public WoWCharacterModel getAllInfos(string realm, string name)
        {
            WoWCharacterModel character = new WoWCharacterModel();
            
            character.deaths = 0;
            character.facepalmed = 0;
            character.level = 0;
            character.questsDone = 0;
            character.fishCaught = 0;
            try {
                var client = new WebClient();
                client.UseDefaultCredentials = true;         
                client.QueryString.Add("apikey", "ak8a3kw6ksu5y89t8bx5ct8f4kp5uzv5");
                client.QueryString.Add("fields", "statistics");
                client.QueryString.Add("locale", "en_GB");
                string result = client.DownloadString("https://eu.api.battle.net/wow/character/" + realm + "/" + name);
                dynamic obj = JsonConvert.DeserializeObject(result);
                var value = obj["statistics"]["subCategories"];
                character.level = obj["level"];
                foreach (var subcategory in value)
                {
                    if (subcategory["name"].ToString() == "Skills")
                        character.fishCaught = (subcategory["subCategories"][0]["statistics"][7]["quantity"]);
                    if (subcategory["name"].ToString() == "Quests")
                        character.questsDone = (subcategory["statistics"][0]["quantity"]);
                    if (subcategory["name"].ToString() == "Social")
                        character.facepalmed = (subcategory["statistics"][1]["quantity"]);
                    if (subcategory["name"].ToString() == "Deaths")
                        character.deaths = (subcategory["statistics"][0]["quantity"]);
                }
            }
            catch
            {
                return (character);
            }
            return (character);
        }

        public int   getNewCharacterStat(string realm, string name)
        {
            if (index == 1)
                return getDeaths(realm, name);
            if (index == 2)
                return getFacepalmed(realm, name);
            if (index == 3)
                return getLevel(realm, name);
            if (index == 4)
                return getQuestsDone(realm, name);
            if (index == 5)
                return getFishCaught(realm, name);
            return getLevel(realm, name);
        }

        public int   getCurrentCharacterStat(WoWCharacterModel character)
        {
            if (index == 1)
                return character.deaths;
            if (index == 2)
                return character.facepalmed;
            if (index == 3)
                return character.level;
            if (index == 4)
                return character.questsDone;
            if (index == 5)
                return character.fishCaught;
            return character.level;
        }

        public void     updateCharacter(int stat, WoWCharacterModel character, AreaDbContext DB)
        {
            if (index == 1)
                character.deaths = stat;
            if (index == 2)
                character.facepalmed = stat;
            if (index == 3)
                character.level = stat;
            if (index == 4)
                character.questsDone = stat;
            if (index == 5)
                character.fishCaught = stat;
            DB.wowcharactermodels.Update(character);
            DB.SaveChanges();
        }

        public  WoWCharacterModel findCharacterModel(string realm, string name, AreaDbContext DB)
        {
            foreach(var character in DB.wowcharactermodels)
            {
                 if (character.realm == realm && character.name == name)
                    return (character);
            }
            return (null);
        }

        public bool   checkAndUpdateCharacter(WoWCharacterModel charact, AreaDbContext DB)
        {
            int stat = getNewCharacterStat(charact.realm, charact.name);

            if (stat > getCurrentCharacterStat(charact))
            {
                Console.WriteLine(charact.name + " stat: " + stat + " current_stat : " + getCurrentCharacterStat(charact));                
                updateCharacter(stat, charact, DB);
                return (true);
            }
            return (false);
        }

        public string   getCharacterAction(WoWCharacterModel character)
        {
            if (index == 0)
                return "New character: " + character.name + "!";
            if (index == 1)
                return "My character " + character.name + " died " + character.deaths + " times!";
            if (index == 2)
                return "My character " + character.name + " facepalmed " + character.facepalmed + " times!";
            if (index == 3)
                return "My character " + character.name + " level up! Now level " + character.level + "!";
            if (index == 4)
                return "My character " + character.name + " has accomplished new quests. Total:" + character.questsDone + "!";
            if (index == 5)
                return "My character " + character.name + " went fishing! He now has caught " + character.fishCaught + " fish!";
            return "";
        }

         public bool alreadyExists(List<WoWCharacterModel> existing_characters, WoWCharacterModel character)
        {
            foreach (var existing_character in existing_characters)
            {
                if (existing_character.realm == character.realm && existing_character.name == character.name)
                    return (true);
            }
            return (false);
        }

        public string   getAction()
        {
            if (index == 0)
                return "ameno";
            if (index == 1)
                return "the sound of silence";
            if (index == 2)
                return "in the end linkin park";
            if (index == 3)
                return "live to win";
            if (index == 4)
                return "eye of the tiger";
            if (index == 5)
                return "a la peche";
            return "ameno";
        }

        public void     postReaction(string reaction)
        {

        }  
    }
}