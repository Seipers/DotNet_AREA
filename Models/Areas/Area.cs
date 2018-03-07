using System;
using Microsoft.EntityFrameworkCore;

namespace Area.Models
{
    public class AREA
    {
        public int      id { get; set; }   
        public string   username {get; set;}
        public int  type {get; set;}
        public int  index_action {get; set;}
        public int  index_reaction {get; set;}
        public string  last_event {get; set;}
    }
}