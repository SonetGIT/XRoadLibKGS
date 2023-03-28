using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SourcesAPI.Models.RequestTypes
{
    public class SaveNotPaymentF20Request
    {
        public string OrderPaymentId { get; set; }
        public DateTime RegDate { get; set; }
        public string ReasonId { get; set; }
    }
}