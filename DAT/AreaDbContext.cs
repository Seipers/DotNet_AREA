using System;
using Microsoft.EntityFrameworkCore;
using Area.Models;

namespace Area.DAT
{
    public class AreaDbContext : DbContext
    {
        public AreaDbContext(DbContextOptions<AreaDbContext> opt) : base(opt) {
        }
        public DbSet<User> users { get; set; }        
        public DbSet<Token> tokens { get; set; }   
        public DbSet<User_role> user_roles {get; set;}
        public DbSet<WoWCharacterModel> wowcharactermodels {get; set;}
        public DbSet<AREA> areas {get; set;}
        public DbSet<AreaType> areatypes {get; set;}
    }
    public class AreaDbThreadContext : DbContext
    {
        public AreaDbThreadContext(DbContextOptions<AreaDbThreadContext> opt) : base(opt) {
        }
        public DbSet<User> users { get; set; }        
        public DbSet<Token> tokens { get; set; }   
        public DbSet<User_role> user_roles {get; set;}
        public DbSet<WoWCharacterModel> wowcharactermodels {get; set;}
        public DbSet<AREA> areas {get; set;}
        public DbSet<AreaType> areatypes {get; set;}
        
    }
}