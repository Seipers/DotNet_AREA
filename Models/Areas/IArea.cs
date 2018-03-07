using System;
using System.Collections.Generic;
using Area.DAT;
using Area.Models;

namespace Area.Models
{
    public interface IArea
    {
      void  run(AreaDbContext DB);
      bool  isAvailable();
    }
}