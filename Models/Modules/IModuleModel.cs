using System;
using System.Collections.Generic;
using Area.Models;

namespace Area.Models
{
    public interface IModuleModel
    {
        string  getAction();
        void    postReaction(string reaction);  
    }
}