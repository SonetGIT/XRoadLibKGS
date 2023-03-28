using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;

namespace WebEF.Contracts
{
    public interface IGetUnemployeeStatus
    {
        [XRoadService("GetUnemployeeStatus")]
        [XRoadTitle("en", "GetUnemployeeStatus service")]
        [XRoadNotes("en", "")]
        ResponseTypes.GetUnemployeeStatusResponse GetUnemployeeStatus(RequestTypes.PINRequest request);
    }
}
