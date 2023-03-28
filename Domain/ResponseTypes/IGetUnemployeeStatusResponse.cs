using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ResponseTypes
{
    public interface IGetUnemployeeStatusResponse
    {
        int RegStatus { get; set; }
    }
}
