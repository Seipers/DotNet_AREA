using System;
using Area.DAT;
using Area.Models;
using System.Collections.Generic;

namespace Area.Models
{
    public class AreaFactory
    {
        public static IArea create(AREA area, AreaDbContext DB)
        {
            if (area.type == 1)
                return new WoWSpotifyArea(area, DB);
            if (area.type == 2)
                return new WoWTwitterArea(area, DB);
            if (area.type == 3)
                return new GithubFacebookArea(area, DB);
            if (area.type == 4)
                return new SpotifyTwitterArea(area, DB);
            if (area.type == 5)
                return new TwitterSpotifyArea(area, DB);
            if (area.type == 6)
                return new FacebookTwitterArea(area, DB);
            if (area.type == 7)
                return new FacebookSpotifyArea(area, DB);
            return (null);
        }
    }
}