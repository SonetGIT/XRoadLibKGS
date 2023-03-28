using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;

namespace WebEF.Contracts
{
    public interface IAdoptedChildrenReport
    {
        [XRoadService("AdoptedChildrenReport")]
        [XRoadTitle("en", "AdoptedChildrenReport service")]
        [XRoadNotes("en", "")]
        ResponseTypes.AdoptedChildrenReportResponse AdoptedChildrenReport(RequestTypes.ChildRequest request);
    }
}
