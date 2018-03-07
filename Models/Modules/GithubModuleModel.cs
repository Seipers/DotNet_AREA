using Octokit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Area.Models
{
    public class GithubModuleModel : IModuleModel
    {
        private string  access_token;
        private int  index;
        public  GithubModuleModel(string token, int index)
        {
            this.index = index;
            this.access_token = token;
        }

        public void     setAccessToken(string token)
        {
            this.access_token = token;
        }

        public string   getAccessToken()
        {
            return (this.access_token);
        }

        public async Task<Dictionary<string, string>> getPublicInfos()
        {
            var client = new GitHubClient(new ProductHeaderValue("AreaNet"));
            var tokenAuth = new Credentials(access_token);
            client.Credentials = tokenAuth;
            var user = await client.User.Current(); //User.Get("name") for any public github profile
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("followers_nb", user.Followers.ToString());
            dict.Add("following_nb", user.Following.ToString());
            dict.Add("login", user.Login);
            dict.Add("location", user.Location);
            dict.Add("private_repo_nb", user.OwnedPrivateRepos.ToString());
            dict.Add("public_repo_nb", user.PublicRepos.ToString());
            dict.Add("company", user.Company);
            dict.Add("name", user.Name);
            dict.Add("bio", user.Bio);
            return (dict);
        }

        public async Task<Dictionary<int, string>> getActions()
        {
            var client = new GitHubClient(new ProductHeaderValue("AreaNet"));
            var tokenAuth = new Credentials(access_token);
            client.Credentials = tokenAuth;
            var user = await client.User.Current(); //User.Get("name") for any public github profile
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, "My new number of followers: " + user.Followers.ToString());
            dict.Add(1, "My new number of following: " + user.Following.ToString());
            dict.Add(2, "My user login: " + user.Login);
            dict.Add(3, "My new location: " + user.Location);
            dict.Add(4, "My new number of private repositories: " + user.OwnedPrivateRepos.ToString());
            dict.Add(5, "My new number of plubic repositories: " + user.PublicRepos.ToString());
            dict.Add(6, "My new company: " +  user.Company);
            dict.Add(7, "My new name: " + user.Name);
            dict.Add(8, "My new bio: " + user.Bio);
            return (dict);
        }

        public async void createRepo(string name)
        {
            //not functional
            var client = new GitHubClient(new ProductHeaderValue("AreaNet"));
            var tokenAuth = new Credentials(access_token);
            client.Credentials = tokenAuth;
            var new_rep = new NewRepository(name)
            {
                AutoInit = true
            };
            var repository = await client.Repository.Create(new_rep);
        }

        public string   getAction()
        {
            return (getActions().Result[this.index]);
        }

        public void     postReaction(string reaction)
        {

        }
    }
}