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
    public interface ISaveBalagaSuyunchu
    {
        [XRoadService("SaveBalagaSuyunchu")]
        [XRoadTitle("en", "SaveBalagaSuyunchu service")]
        [XRoadNotes("en", "SaveBalagaSuyunchu")]
        BalagaSuyunchuResponse SaveBalagaSuyunchu(BirthPaymentApplication request);
    }
}
