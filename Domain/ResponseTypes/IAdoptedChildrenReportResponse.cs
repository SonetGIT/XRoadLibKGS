using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ResponseTypes
{
    public interface IAdoptedChildrenReportResponse
    {
        //By age
        IAdoptedChildrenReportItem[] ByAge { get; set; }

        //Nationalities
        IAdoptedChildrenReportItem[] ByNationalities { get; set; }

        //By geography
        IAdoptedChildrenReportItem[] ByGeography { get; set; }

    }
    public interface IAdoptedChildrenReportItem
    {
        int No { get; set; }
        string Name { get; set; }
        int Boys { get; set; }
        int Girls { get; set; }
        int Total { get; set; }
    }
}
