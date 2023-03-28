
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SourcesAPI.Models.ResponseTypes
{
    public class GetNewOldRecipientsByYearMonthResponse
    {
        public string[] NewPINs { get; set; }
        public string[] ExpiredPINs { get; set; }
    }
}