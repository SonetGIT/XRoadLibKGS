using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebEF.Models
{
    public class ServiceDetail
    {
        public int Id { get; set; }
        public Guid? ServiceDescriptionId { get; set; }

        [ForeignKey("ServiceCode")]
        public int? ServiceCodeId { get; set; }
        public virtual ServiceCode ServiceCode { get; set; }
    }
}
