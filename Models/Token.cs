using System;
using Microsoft.EntityFrameworkCore;

namespace Area.Models
{
    public class Token
    {
        public int      id { get; set; }
        public string   type {get; set;}
        public string   username {get; set;}
        public string   value {get; set;}
    }
}