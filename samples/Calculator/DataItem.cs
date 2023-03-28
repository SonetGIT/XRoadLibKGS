using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculator
{
    public class DataItem
    {
        public string Name { get; set; }
        public Type DataType { get; set; }
        public bool isMethod { get; set; }
        public DataItem InputParamType { get; set; }
    }
}
