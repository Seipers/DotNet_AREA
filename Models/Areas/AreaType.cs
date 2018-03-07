using System;
using System.Collections.Generic;
using System.Linq;
using Area.DAT;
using Microsoft.EntityFrameworkCore;

namespace Area.Models
{
    public class AreaType
    {
        public int      id {get; set;}   
        public string   name {get; set;}
        public string   description {get; set;}
        public string   create_url {get; set;}
        public string   delete_url {get; set;}
        public string   img_url {get; set;}

        public static AreaType  findAreaTypeById(AreaDbContext DB, int id)
        {
            List<AreaType> areatypes = DB.areatypes.ToList();
            foreach (var areatype in areatypes)
            {
                if (areatype.id == id)
                    return (areatype);
            }
            return (null);
        }
    }
}