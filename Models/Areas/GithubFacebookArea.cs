using System;
using System.Collections.Generic;
using Area.Models;
using Area.DAT;
using System.Linq;

namespace Area.Models
{
    public class GithubFacebookArea : IArea
    {
        private GithubModuleModel Github;
        private FacebookModule Facebook;
        private AREA area;

        public GithubFacebookArea(AREA area, AreaDbContext DB)
        {
            this.area = area;
            string github_access_token = "";
            string facebook_access_token = "";
            foreach (var token in DB.tokens)
            {
                if (token.type == "Github" && token.username == area.username)
                    github_access_token = token.value;
                if (token.type == "Facebook" && token.username == area.username)
                    facebook_access_token = token.value;
            }
            Github = new GithubModuleModel(github_access_token, area.index_action);
            Facebook = new FacebookModule(facebook_access_token, area.index_reaction);
        }
        
        public void  run(AreaDbContext DB)
        {
            string action = Github.getAction();
            
            if (action != area.last_event)
            {
                Facebook.PostOnFacebook(action);
                area.last_event = action;
                DB.areas.Update(area);
                DB.SaveChanges();
            }
        }

        public bool isAvailable()
        {
            return (!String.IsNullOrEmpty(Github.getAccessToken()) && !String.IsNullOrEmpty(Facebook.getAccessToken()));
        }
    }
}