using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ResponseTypes
{
    public class GetNewOldRecipientsByYearMonthResponse
    {
        public int TotalNew
        {
            get
            {
                return NewPINs != null ? NewPINs.Length : 0;
            }
        }
        public int TotalExpired
        {
            get
            {
                return ExpiredPINs != null ? ExpiredPINs.Length : 0;
            }
        }
        public string[] NewPINs { get; set; }
        public string[] ExpiredPINs { get; set; }
    }
}
