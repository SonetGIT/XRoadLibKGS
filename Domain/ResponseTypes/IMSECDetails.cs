using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ResponseTypes
{
    public interface IMSECDetails
    {
        string OrganizationName { get; set; }
        DateTime ExaminationDate { get; set; }
        string ExaminationType { get; set; }
        string DisabilityGroup { get; set; }
        DateTime From { get; set; }
        DateTime To { get; set; }
        DateTime TimeOfDisability { get; set; }
        string ReExamination { get; set; }
        string StatusCode { get; set; }
    }
}
