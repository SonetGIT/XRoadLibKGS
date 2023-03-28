using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEF.Contracts.BalagaSuyunchu;
using WebEF.Contracts.RequestTypes;
using WebEF.Contracts.ResponseTypes;
using XRoadLib.Attributes;

namespace WebEF.Contracts
{
    public interface IGetBalagaSuyunchu
    {
        [XRoadService("GetBalagaSuyunchu")]
        [XRoadTitle("en", "GetBalagaSuyunchu service")]
        [XRoadNotes("en", "GetBalagaSuyunchu")]
        BirthPaymentApplication GetBalagaSuyunchu(BirthPaymentApplication request);
    }
}
