using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponseTypes
{
    public interface IGetNewOldRecipientsByYearMonthResponse
    {
        string[] NewPINs { get; set; }
        string[] ExpiredPINs { get; set; }
    }
}
