using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;

namespace WebEF.Contracts
{
    public interface IMSECDetails
    {
        [XRoadService("MSECDetails")]
        [XRoadTitle("en", "MSECDetails service")]
        [XRoadNotes("en", "")]
        ResponseTypes.MSECDetailsResponse MSECDetails(RequestTypes.PINRequest request);
    }
}
