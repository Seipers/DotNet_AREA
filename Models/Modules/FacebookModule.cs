using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Facebook;
using Newtonsoft.Json;

namespace Area.Models
{
    public class FacebookModule : IModuleModel
    {
        private FacebookClient _fb = new FacebookClient(); 
        private int _index;

        private Dictionary<int, Func<string>> actions = new Dictionary<int, Func<string>>();

        public FacebookModule(string accessToken, int index)
        {
            _fb.AccessToken = accessToken;
            _index = index;

            actions.Add(0, new Func<string>(GetEmail));
            actions.Add(1, new Func<string>(GetAgeRange));
            actions.Add(2, new Func<string>(GetCover));
            actions.Add(3, new Func<string>(GetFirstName));
            actions.Add(4, new Func<string>(GetLastName));
            actions.Add(5, new Func<string>(GetFullName));
            actions.Add(6, new Func<string>(GetGender));
            actions.Add(7, new Func<string>(GetLocale));
            actions.Add(8, new Func<string>(GetPicture));
            actions.Add(9, new Func<string>(GetUserLastPosts));
            actions.Add(10, new Func<string>(GetUserLastPostsEpured));
        }

        public string   getAccessToken()
        {
            return (_fb.AccessToken);
        }
        public void setIndex(int idx)
        {
            _index = idx;
        }
        
        public void PostOnFacebook(string toPost)
        {
            try {
                dynamic parameters = new ExpandoObject();
                parameters.message = toPost;

                dynamic result = _fb.Post("me/feed", parameters);
                var id = result.id;
            }
            catch
            {
                Console.WriteLine("Could not post on facebook");
            }
        }

        public string GetEmail()
        {
            string field = "email";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
            var obj = new Object();
            if (tmp.TryGetValue("email", out obj))
                return "My current email on facebook is: " + obj.ToString();
            return "No email record";
        }

        public string GetCover()
        {
            string field = "cover";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
            var obj = new Object();
            if (tmp.TryGetValue("cover", out obj))
            {
                var tmp2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(tmp["cover"].ToString());
                if (tmp2.TryGetValue("source", out obj))
                    return "My current cover url is: " + tmp2["source"].ToString();
            }
            return ("No Cover Url has been found");
        }

        public string GetFullName()
        {
            string field = "name";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ToString());
            
            string obj;
            if (tmp.TryGetValue("name", out obj))
                return "My current name on facebook is: " + obj;
            return "No Name has been founded";
        }

        public string GetFirstName()
        {
            string field = "first_name";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ToString());
            
            string obj;
            if (tmp.TryGetValue("first_name", out obj))
                return "My current first name on facebook is: " + obj;
            return "No FirstName has been founded";
        }

        public string GetLastName()
        {
            string field = "last_name";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ToString());
            
            string obj;
            if (tmp.TryGetValue("last_name", out obj))
                return "My current last name on facebook is: " + obj;
            return "No Last Name has been founded";
        }

        public string GetAgeRange()
        {
            string field = "age_range";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
            var obj = new Object();
            if (tmp.TryGetValue("age_range", out obj))
            {
                var tmp2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(tmp["age_range"].ToString());
                if (tmp2.TryGetValue("min", out obj))
                    return "My current: minimum age on facebook is: " + tmp2["min"].ToString();
            }
            return "No minimum age has been found";
        }

        public string GetGender()
        {
            string field = "gender";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ToString());
            
            string obj;
            if (tmp.TryGetValue("gender", out obj))
                return "My current gender on facebook is: " + obj;
            return "No Gender has been founded";
        }

        public string GetLocale()
        {
            string field = "locale";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.ToString());
            
            string obj;
            if (tmp.TryGetValue("locale", out obj))
                return "My current location on facebook is: " + obj;
            return "No Locale has been found";
        }

        public string GetPicture()
        {
            string field = "picture";
            var parameters = new Dictionary<string, object>();
            parameters["fields"] = field;

            var result = _fb.Get("me", parameters);
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
            var obj = new Object();
            if (tmp.TryGetValue("picture", out obj))
            {
                var tmp2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(tmp["picture"].ToString());
                if (tmp2.TryGetValue("data", out obj))
                {
                    var tmp3 = JsonConvert.DeserializeObject<Dictionary<string, object>>(tmp2["data"].ToString());
                    if (tmp3.TryGetValue("url", out obj))            
                        return "My current url picture on facebook is: " + tmp3["url"].ToString();
                }
            }
            return ("No Url for your picture has been found");            
        }

        public string GetUserLastPosts()
        {
            var result = _fb.Get("me/feed");
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
            var obj = new object();
            if (tmp.TryGetValue("data", out obj))
            {
                var tmp2 = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(tmp["data"].ToString());
                if (tmp2.Count == 0) return "No last post has been found";
                var dic = tmp2[0];
                string str, str2, str3;
                if (dic.TryGetValue("message", out str) && dic.TryGetValue("story", out str2) && dic.TryGetValue("created_time", out str3))
                    return "Most recent post on facebook: " + str3 + " " + str + " " + str2;
                else if (dic.TryGetValue("message", out str) && !dic.TryGetValue("story", out str2) && dic.TryGetValue("created_time", out str3))
                    return "Most recent post on facebook: " + str3 + " " + str;
                else if (!dic.TryGetValue("message", out str) && dic.TryGetValue("story", out str2) && dic.TryGetValue("created_time", out str3))
                    return "Most recent post on facebook: " + str3 + " " + str2;
            }
            return "No Last Post has been found";
        }

        public string GetUserLastPostsEpured()
        {
            var result = _fb.Get("me/feed");
            var tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
            var obj = new object();
            if (tmp.TryGetValue("data", out obj))
            {
                var tmp2 = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(tmp["data"].ToString());
                if (tmp2.Count == 0) return "No last post has been found";
                var dic = tmp2[0];
                string str, str2;
                if (dic.TryGetValue("message", out str) && dic.TryGetValue("story", out str2))
                    return str + " " + str2;
                else if (dic.TryGetValue("message", out str) && !dic.TryGetValue("story", out str2))
                    return str;
                else if (!dic.TryGetValue("message", out str) && dic.TryGetValue("story", out str2))
                    return str2;
            }
            return "No Last Post has been found";
        }

        public string getAction()
        {
            return actions[_index]();
        }

        public void postReaction(string reaction)
        {
            throw new NotImplementedException();
        }
    }
}
