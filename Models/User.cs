using System;
using Microsoft.EntityFrameworkCore;

namespace Area.Models
{
    public class User
    {
        public int    id { get; set; }   
        public string   username {get; set;}
        public string   password {get; set;}
        public string   email { get; set; }
        public bool     enabled {get; set;}
    }
}