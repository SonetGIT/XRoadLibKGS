using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEF.Contracts.RequestTypes;
using WebEF.Contracts.ResponseTypes;
using XRoadLib.Attributes;

namespace WebEF.Contracts
{
    public interface ISetPaymentInfo
    {
        [XRoadService("SetPaymentInfo")]
        [XRoadTitle("en", "SetPaymentInfo service")]
        [XRoadNotes("en", "SetPaymentInfo")]
        SetPaymentInfoResponse SetPaymentInfo(SetPaymentInfoRequest request);
    }
}
