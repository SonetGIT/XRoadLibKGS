using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SourcesAPI.Models.ResponseTypes.Notifications
{
    public class NotificationModelResponse
    {
        public bool Result { get; set; } = false;
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
    }
}