using XRoadLib.Attributes;
using XRoadLib.Serialization;

namespace WebEF.Contracts.ResponseTypes
{
    public class GetRecipientsResponse : XRoadSerializable, Domain.ResponseTypes.IGetNewOldRecipientsByYearMonthResponse
    {
        [XRoadXmlArray(Order = 1)]
        public string[] NewPINs { get; set; }
        [XRoadXmlArray(Order = 2)]
        public string[] ExpiredPINs { get; set; }
    }
}
