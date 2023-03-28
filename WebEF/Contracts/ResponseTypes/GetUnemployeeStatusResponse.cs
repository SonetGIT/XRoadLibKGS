using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class GetUnemployeeStatusResponse : XRoadSerializable, Domain.ResponseTypes.IGetUnemployeeStatusResponse
    {
        [XRoadXmlElement(Order = 1)]
        public int RegStatus { get; set; }
    }
}
