using WebEF.Contracts.RequestTypes;
using WebEF.Contracts.ResponseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Headers;

namespace WebEF.Contracts
{
    public interface IGetRecipients
    {
        [XRoadService("GetRecipients")]
        [XRoadTitle("en", "GetRecipients service")]
        [XRoadNotes("en", "")]
        GetRecipientsResponse GetRecipients(YearMonthRequest request);
    }
}
