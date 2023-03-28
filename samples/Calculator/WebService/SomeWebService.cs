using Calculator.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculator.WebService
{
    public class SomeWebService : ISome
    {
        public int Add(int x)
        {
            return 100;
        }
    }
}
