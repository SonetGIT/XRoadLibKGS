using xRoadWeb.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xRoadWeb.Services
{
    public class SomeWebService : ISome
    {
        public int Add(int x)
        {
            return 100;
        }
    }
}
