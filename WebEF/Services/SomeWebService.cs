using WebEF.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebEF.Contracts.ResponseTypes;
using WebEF.Contracts.RequestTypes;
using System.Net;
using System.IO;
using Microsoft.Extensions.Options;
using WebEF.Models;

namespace WebEF.Services
{
    public class SomeWebService : ISome
    {
        private readonly IOptions<AppSettings> _appSettings;
        
        public GetRecipientsResponse GetRecipients(YearMonthRequest request)
        {
            if (string.IsNullOrEmpty(_appSettings.Value.SourcesAPIHost))
            {
                throw new ApplicationException("SourcesAPIHost not found in appSetting.json!");
            }
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:3393/api/CISSA/GetNewOldRecipientsByYearMonth");
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(request);

                streamWriter.Write(json);
                streamWriter.Flush();
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var responseText = "";
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GetRecipientsResponse>(responseText);
        }
    }
}
