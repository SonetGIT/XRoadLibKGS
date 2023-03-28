using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebEF.Models
{
    public class TransmitHistoryItem
    {
        public int Id { get; set; }

        [Display(Name = "Дата и время операции")]
        public DateTime EntryTime { get; set; }

        [Display(Name = "Длительность операции")]
        public double OperationDuration { get; set; } = 0;

        [NotMapped]
        [Display(Name = "Дата и время конца операции")]
        public DateTime EndTime { get { return EntryTime.AddMilliseconds(OperationDuration); } }

        [Display(Name = "Успешно ли выполнена операция?")]
        public bool IsSuccess { get; set; }

        [Display(Name = "Текст ошибки")]
        public string ErrorMessage { get; set; }

        [Display(Name = "Размер входящих данных")]
        public double? InputSize { get; set; } = 0;

        [Display(Name = "Размер исходящих данных")]
        public double? OutputSize { get; set; } = 0;

        public int? ServiceDetailId { get; set; }

        public string c_objectType { get; set; }
        public string c_xRoadInstance { get; set; }
        [Display(Name = "Тип организации")]
        public string c_memberClass { get; set; }
        [Display(Name = "Код организации")]
        public string c_memberCode { get; set; }
        [Display(Name = "Код подсистемы")]
        public string c_subsystemCode { get; set; }

        [Display(Name = "Кол-во строк в результате")]
        public int? OutputRows { get; set; } = 0;
    }
}
