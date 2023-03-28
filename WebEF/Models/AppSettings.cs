using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebEF.Models
{
    public class AppSettings
    {
        public string SourcesAPIHost { get; set; }
        public string CISSA_REST_APIHost { get; set; }
        public string ISRT_REST_APIHost { get; set; }
        public string BalagaSuyunchu_REST_ApiHost { get; set; }
    }
}
