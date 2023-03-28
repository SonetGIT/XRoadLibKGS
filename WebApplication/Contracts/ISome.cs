using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;

namespace WebApplication.Contracts
{
    public interface ISome
    {
        [XRoadService("Some")]
        [XRoadTitle("en", "Calculation service")]
        [XRoadNotes("en", "Performs specified operation on two user provided integers and returns the result.")]
        int Add(int x);
    }
}
