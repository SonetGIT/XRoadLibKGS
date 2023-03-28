using Intersoft.CISSA.DataAccessLayer.Model;
using Intersoft.CISSA.DataAccessLayer.Model.Context;
using Intersoft.CISSA.DataAccessLayer.Model.Documents;
using Intersoft.CISSA.DataAccessLayer.Model.Query.Builders;
using Intersoft.CISSA.DataAccessLayer.Model.Query.Sql;
using Intersoft.CISSA.DataAccessLayer.Model.Workflow;
using SourcesAPI.Models.RequestTypes;
using SourcesAPI.Models.RequestTypes.Notifications;
using SourcesAPI.Models.ResponseTypes;
using SourcesAPI.Models.ResponseTypes.Notifications;
using SourcesAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace SourcesAPI.Controllers
{
    public class CISSAController : ApiController
    {
        private Guid appDefId = new Guid("{04D25808-6DE9-42F5-8855-6F68A94A224C}");
        private Guid appPilotDefId = new Guid("{4B8EA3F0-D536-4A59-9D2A-41872D63103C}");//Заявление на балага-суйунчу
        private Guid personDefId = new Guid("{6F5B8A06-361E-4559-8A53-9CB480A9B16C}");
        private Guid assignmentDefId = new Guid("{5D599CE4-76C5-4894-91CC-4EB3560196CE}");
        private Guid accountDefId = new Guid("{81C532F6-F5B0-4EFC-8305-44E864E778D3}");
        private Guid postOrderDefId = new Guid("{19EA8D75-2EE7-42CA-BE3B-D7E41F343DDD}");
        private Guid f10DefId = new Guid("{A8B9DAB6-CDEA-44A5-BAF5-D19F1879B9A6}");
        private Guid f20DefId = new Guid("{A0370B35-11A8-41D2-96AF-AB6C956DE5F1}");
        private Guid dszUserId = new Guid("{64C178E7-3F82-4E44-A7A1-FECB5FE693FF}");
        private Guid onPaymentStateTypeId = new Guid("{3DB4DB00-3A7F-4228-A9A3-A413B85C18B4}");
        private Guid refusedStateTypeId = new Guid("{CA1157A0-FDDF-4C90-9692-7CDB47CCC7C2}");
        private Guid factPaymentInfoDefId = new Guid("{8E9DF822-5052-45A1-8561-ADEB657FCB26}");
        [HttpPost]
        [ResponseType(typeof(GetActivePaymentsByPINResponse))]
        public IHttpActionResult GetActivePaymentsByPIN([FromBody]PINRequest request)
        {
            var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, dszUserId), new DataContext(null));
            try
            {
                var qb = new QueryBuilder(appDefId, context.UserId);
                qb.Where("Applicant").Include("PIN").Eq(request.PIN).End().And("&State").Neq(refusedStateTypeId);
                var query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
                var personSrc = query.JoinSource(query.Source, personDefId, SqlSourceJoinType.Inner, "Applicant");
                query.AddAttribute(query.Source, "AssignFrom");
                query.AddAttribute(query.Source, "AssignToMax");
                query.AddAttribute(query.Source, "PaymentType");
                query.AddAttribute(query.Source, "PaymentSum");
                query.AddAttribute(personSrc, "LastName");
                query.AddAttribute(personSrc, "FirstName");
                query.AddAttribute(personSrc, "MiddleName");
                query.AddAttribute(query.Source, "&OrgId");
                query.AddAttribute(query.Source, "&Id");

                query.AddOrderAttribute(query.Source, "AssignFrom", false);

                var result = new GetActivePaymentsByPINResponse();
                Guid appId = Guid.Empty;
                using (var reader = new SqlQueryReader(context.DataContext, query))
                {
                    if (reader.Read())
                    {
                        result.StartDate = reader.IsDbNull(0) ? DateTime.MinValue : reader.GetDateTime(0);
                        result.EndDate = reader.IsDbNull(1) ? DateTime.MaxValue : reader.GetDateTime(1);
                        result.PaymentTypeName = reader.IsDbNull(2) ? "" : context.Enums.GetValue(reader.GetGuid(2)).Value;
                        result.PaymentSize = reader.IsDbNull(3) ? 0 : reader.GetDecimal(3);
                        result.LastName = reader.IsDbNull(4) ? "" : reader.GetString(4);
                        result.FirstName = reader.IsDbNull(5) ? "" : reader.GetString(5);
                        result.MiddleName = reader.IsDbNull(6) ? "" : reader.GetString(6);
                        result.OrganizationName = reader.IsDbNull(7) ? "" : context.Orgs.GetOrgName(reader.GetGuid(7));
                        appId = reader.GetGuid(8);
                    }
                }
                if (appId != Guid.Empty)
                {
                    var deps = GetAssignments(context, appId);
                    if (deps.Count > 0)
                        result.Dependants = deps.ToArray();
                    return Ok(result);
                }
                else
                {
                    qb = new QueryBuilder(assignmentDefId, context.UserId);
                    qb.Where("Person").Include("PIN").Eq(request.PIN).End();
                    query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
                    personSrc = query.JoinSource(query.Source, personDefId, SqlSourceJoinType.Inner, "Person");
                    query.AddAttribute(query.Source, "EffectiveDate");
                    query.AddAttribute(query.Source, "ExpiryDateMax");
                    query.AddAttribute(query.Source, "PaymentType");
                    query.AddAttribute(query.Source, "Amount");
                    query.AddAttribute(personSrc, "LastName");
                    query.AddAttribute(personSrc, "FirstName");
                    query.AddAttribute(personSrc, "MiddleName");
                    query.AddAttribute(query.Source, "&OrgId");

                    query.AddOrderAttribute(query.Source, "EffectiveDate", false);

                    using (var reader = new SqlQueryReader(context.DataContext, query))
                    {
                        if (reader.Read())
                        {
                            result.StartDate = reader.IsDbNull(0) ? DateTime.MinValue : reader.GetDateTime(0);
                            result.EndDate = reader.IsDbNull(1) ? DateTime.MaxValue : reader.GetDateTime(1);
                            result.PaymentTypeName = reader.IsDbNull(2) ? "" : context.Enums.GetValue(reader.GetGuid(2)).Value;
                            result.PaymentSize = reader.IsDbNull(3) ? 0 : reader.GetDecimal(3);
                            result.LastName = reader.IsDbNull(4) ? "" : reader.GetString(4);
                            result.FirstName = reader.IsDbNull(5) ? "" : reader.GetString(5);
                            result.MiddleName = reader.IsDbNull(6) ? "" : reader.GetString(6);
                            result.OrganizationName = reader.IsDbNull(7) ? "" : context.Orgs.GetOrgName(reader.GetGuid(7));
                            return Ok(result);
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        private List<string> GetAssignments(WorkflowContext context, Guid appId)
        {
            var dependants = new List<string>();
            var query = new SqlQuery(context.Documents.LoadById(appId), "Assignments", context.UserId, "asgn", context.DataContext);
            var personSrc = query.JoinSource(query.Source, personDefId, SqlSourceJoinType.Inner, "Person");
            query.AddAttribute(personSrc, "PIN");
            using (var reader = new SqlQueryReader(context.DataContext, query))
            {
                while (reader.Read())
                {
                    dependants.Add(reader.IsDbNull(0) ? "" : reader.GetString(0));
                }
            }
            return dependants;
        }

        [ResponseType(typeof(GetNewOldRecipientsByYearMonthResponse))]
        [HttpPost]
        public IHttpActionResult GetNewOldRecipientsByYearMonth([FromBody]YearMonthRequest request)
        {
            var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, dszUserId), new DataContext(null));
            try
            {
                var fd = new DateTime(request.Year, request.Month, 1);
                var ld = fd.AddMonths(1).AddDays(-1);
                var responseObj = new GetNewOldRecipientsByYearMonthResponse();
                var qb = new QueryBuilder(appDefId, context.UserId);
                qb.Where("AssignToMax").IsNotNull().And("AssignFrom").Ge(fd).And("AssignFrom").Le(ld);
                var query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
                var personSrc = query.JoinSource(query.Source, personDefId, SqlSourceJoinType.Inner, "Applicant");
                query.AddAttribute(personSrc, "PIN");
                query.AddAttribute(personSrc, "&Id");
                var PINList = new List<string>();
                using (var reader = new SqlQueryReader(context.DataContext, query))
                {
                    while (reader.Read())
                    {
                        var pin = !reader.IsDbNull(0) ? reader.GetString(0) : "";
                        var emptyPIN = reader.GetGuid(1).ToString();
                        PINList.Add(string.IsNullOrEmpty(pin) ? emptyPIN : pin.Trim());
                    }
                }
                responseObj.NewPINs = PINList.ToArray();
                qb = new QueryBuilder(appDefId, context.UserId);
                qb.Where("AssignToMax").Ge(fd).And("AssignToMax").Le(ld);
                query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
                personSrc = query.JoinSource(query.Source, personDefId, SqlSourceJoinType.Inner, "Applicant");
                query.AddAttribute(personSrc, "PIN");
                query.AddAttribute(personSrc, "&Id");
                PINList = new List<string>();
                using (var reader = new SqlQueryReader(context.DataContext, query))
                {
                    while (reader.Read())
                    {
                        var pin = !reader.IsDbNull(0) ? reader.GetString(0) : "";
                        var emptyPIN = reader.GetGuid(1).ToString();
                        PINList.Add(string.IsNullOrEmpty(pin) ? emptyPIN : pin.Trim());
                    }
                }
                responseObj.ExpiredPINs = PINList.ToArray();
                return Ok(responseObj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ResponseType(typeof(SavePaymentF10Response))]
        [HttpPost]
        public IHttpActionResult SavePaymentF10([FromBody]SavePaymentF10Request request)
        {
            var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, dszUserId), new DataContext(null));
            try
            {
                var responseObj = new SavePaymentF10Response();
                var qb = new QueryBuilder(postOrderDefId, context.UserId);
                qb.Where("OrderPayments").Include("&Id").Eq(Guid.Parse(request.OrderPaymentId)).End();
                var query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
                query.AddAttribute("Account");
                var accountId = Guid.Empty;
                using (var reader = new SqlQueryReader(context.DataContext, query))
                {
                    if (reader.Read() && !reader.IsDbNull(0))
                    {
                        accountId = reader.GetGuid(0);
                    }
                }
                if (accountId != Guid.Empty)
                {
                    var docRepo = context.Documents;
                    var f10 = docRepo.New(f10DefId);
                    f10["OrderPayment"] = request.OrderPaymentId;
                    f10["Date"] = request.PayDate;
                    docRepo.Save(f10);
                    docRepo.AddDocToList(f10.Id, docRepo.LoadById(accountId), "ActualPayments");
                    responseObj.PaymentF10Id = f10.Id.ToString();

                    //Check on existing notPayment for this payment period
                    var existingNotPayments = new List<Guid>();
                    qb = new QueryBuilder(f20DefId, context.UserId);
                    qb.Where("OrderPayment").Eq(Guid.Parse(request.OrderPaymentId));
                    query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
                    query.AddAttribute("&Id");
                    query.AddAttribute("Paid");
                    using (var reader = new SqlQueryReader(context.DataContext, query))
                    {
                        while (reader.Read() && !(reader.IsDbNull(1) ? false : reader.GetBoolean(1)))
                        {
                            existingNotPayments.Add(reader.GetGuid(0));
                        }
                    }
                    foreach (var notPaymentId in existingNotPayments)
                    {
                        var f20 = docRepo.LoadById(notPaymentId);
                        f20["Paid"] = true;
                        docRepo.Save(f20);
                    }
                }
                return Ok(responseObj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ResponseType(typeof(SaveNotPaymentF20Response))]
        [HttpPost]
        public IHttpActionResult SaveNotPaymentF20([FromBody]SaveNotPaymentF20Request request)
        {
            var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, dszUserId), new DataContext(null));
            try
            {
                var responseObj = new SaveNotPaymentF20Response();
                var qb = new QueryBuilder(postOrderDefId, context.UserId);
                qb.Where("OrderPayments").Include("&Id").Eq(Guid.Parse(request.OrderPaymentId)).End();
                var query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
                query.AddAttribute("Account");
                var accountId = Guid.Empty;
                using (var reader = new SqlQueryReader(context.DataContext, query))
                {
                    if (reader.Read() && !reader.IsDbNull(0))
                    {
                        accountId = reader.GetGuid(0);
                    }
                }
                if (accountId != Guid.Empty)
                {
                    var docRepo = context.Documents;
                    var f20 = docRepo.New(f20DefId);
                    f20["OrderPayment"] = request.OrderPaymentId;
                    f20["RegDate"] = request.RegDate;
                    f20["Reasons"] = Guid.Parse(request.ReasonId);
                    docRepo.Save(f20);
                    docRepo.AddDocToList(f20.Id, docRepo.LoadById(accountId), "NotPayment");
                    responseObj.PaymentF20Id = f20.Id.ToString();
                }
                return Ok(responseObj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ResponseType(typeof(SetPaymentInfoResponse))]
        [HttpPost]
        public IHttpActionResult SetPaymentInfo([FromBody]SetPaymentInfoRequest request)
        {
            var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, dszUserId), new DataContext(null));
            var onPaymentStateTypeId = new Guid("{3DB4DB00-3A7F-4228-A9A3-A413B85C18B4}");
            try
            {
                var responseObj = new SetPaymentInfoResponse();
                var qb = new QueryBuilder(appPilotDefId, context.UserId);
                qb.Where("Applicant").Include("PIN").Eq(request.PIN).End();
                var query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
                query.AddAttribute("&Id");
                query.AddAttribute("&State");
                Guid appId = Guid.Empty;
                Guid stateId = Guid.Empty;
                using (var reader = new SqlQueryReader(context.DataContext, query))
                {
                    if (reader.Read())
                    {
                        appId = !reader.IsDbNull(0) ? reader.GetGuid(0) : Guid.Empty;
                        stateId = !reader.IsDbNull(1) ? reader.GetGuid(1) : Guid.Empty;
                    }
                }
                if (appId != Guid.Empty)
                {
                    var docRepo = context.Documents;
                    if (stateId != onPaymentStateTypeId)
                    {
                        var state = docRepo.GetDocState(appId);
                        throw new ApplicationException("Данное заявление не установлено к выплате, или список/ведомость не утвержден. Текущий статус заявления: " + (state != null ? state.Type.Name : "пусто"));
                    }
                    var payment = docRepo.New(factPaymentInfoDefId);
                    payment["BalagaSuyunchuApp"] = appId;
                    payment["Sum"] = request.Amount;
                    payment["PaidAt"] = request.PayDate;
                    docRepo.Save(payment);
                    responseObj.Result = true;
                }
                else
                {
                    throw new ApplicationException("Не могу произвести оплату, заявитель с ПИН \"" + request.PIN + "\" не найден!");
                }
                return Ok(responseObj);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //Снятие статусов
        [ResponseType(typeof(NotificationModelResponse))]
        public IHttpActionResult SetExpiredStatuses([FromBody]NotificationRequest request)
        {
            try
            {
                //Назначение
                var apps = DALService.GetExpiredAssignments(request.ForDate, true);

                foreach (var orgApps in apps.GroupBy(x => x.OrgId))
                {
                    var userContext = DALService.GetUserContext(orgApps.Key);

                    foreach (var item in orgApps)
                    {
                        DALService.ApplyCompletionDocs(item.Docs, userContext);
                        DALService.CreateNotificationOfCompletion(request.ForDate, item, userContext, DALService.assignmentDocumentEnumItemId);
                    }
                }

                var assignmentsTotal = apps.Sum(x => x.Amount);
                var assignmentsOrganizationsTotal = apps.GroupBy(x => x.OrgId).Count();

                //Почтовое поручение
                var postOrders = DALService.GetExpiredOrders(request.ForDate, DALService.postOrderDefId, true);

                foreach (var orgOrders in postOrders.GroupBy(x => x.OrgId))
                {
                    var userContext = DALService.GetUserContext(orgOrders.Key);

                    foreach (var item in orgOrders)
                    {
                        DALService.ApplyCompletionDocs(item.Docs, userContext);
                        DALService.CreateNotificationOfCompletion(request.ForDate, item, userContext, DALService.postOrderDocumentEnumItemId);
                    }
                }

                var postOrdersTotal = postOrders.Sum(x => x.Amount);
                var postOrdersOrganizationsTotal = postOrders.GroupBy(x => x.OrgId).Count();

                //Банковское поручение
                var bankOrders = DALService.GetExpiredOrders(request.ForDate, DALService.bankOrderDefId, true);

                foreach (var orgOrders in bankOrders.GroupBy(x => x.OrgId))
                {
                    var userContext = DALService.GetUserContext(orgOrders.Key);

                    foreach (var item in orgOrders)
                    {
                        DALService.ApplyCompletionDocs(item.Docs, userContext);
                        DALService.CreateNotificationOfCompletion(request.ForDate, item, userContext, DALService.bankOrderDocumentEnumItemId);
                    }
                }

                var bankOrdersTotal = bankOrders.Sum(x => x.Amount);
                var bankOrdersOrganizationsTotal = bankOrders.GroupBy(x => x.OrgId).Count();

                var responseObj = new NotificationModelResponse
                {
                    Data = new
                    {
                        assignmentsTotal,
                        assignmentsOrganizationsTotal,
                        postOrdersTotal,
                        postOrdersOrganizationsTotal,
                        bankOrdersTotal,
                        bankOrdersOrganizationsTotal
                    }
                };
                return Ok(responseObj);
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        //Предупреждение об окончании срока назначения
        [ResponseType(typeof(NotificationModelResponse))]
        public IHttpActionResult NotificationOfPreCompletion([FromBody]NotificationRequest request)
        {
            try
            {
                //Назначение
                var apps = DALService.GetExpiredAssignments(request.ForDate.AddMonths(1), true);

                foreach (var orgApps in apps.GroupBy(x => x.OrgId))
                {
                    var userContext = DALService.GetUserContext(orgApps.Key);

                    foreach (var item in orgApps)
                    {
                        DALService.CreateNotificationOfPreCompletion(request.ForDate, item, userContext, DALService.assignmentDocumentEnumItemId);
                    }
                }

                var willCompleteApps = apps.Sum(x => x.Amount);
                var willCompleteAppsOrganizationsTotal = apps.GroupBy(x => x.OrgId).Count();
                var responseObj = new NotificationModelResponse
                {
                    Data = new
                    {
                        willCompleteApps,
                        willCompleteAppsOrganizationsTotal
                    }
                };
                return Ok(responseObj);
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        //Предупреждение об окончании срока паспорта
        [ResponseType(typeof(NotificationModelResponse))]
        public IHttpActionResult NotificationOfPassportExpiration([FromBody]NotificationRequest request)
        {
            try
            {
                //Назначение
                var apps = DALService.GetExpiredPassportAssignments(request.ForDate, true);

                foreach (var orgApps in apps.GroupBy(x => x.OrgId))
                {
                    var userContext = DALService.GetUserContext(orgApps.Key);

                    foreach (var item in orgApps)
                    {
                        DALService.CreateNotificationOfPassportExpiration(request.ForDate, item, userContext, DALService.assignmentDocumentEnumItemId);
                    }
                }

                var expiredPassports = apps.Count;
                var expiredPassportsOrganizations = apps.GroupBy(x => x.OrgId).Count();

                var responseObj = new NotificationModelResponse
                {
                    Data = new
                    {
                        expiredPassports,
                        expiredPassportsOrganizations
                    }
                };
                return Ok(responseObj);
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.GenerateSummaryReport.ReportItem[]))]
        public IHttpActionResult SummaryReport([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.GenerateSummaryReport.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.PoorBenefitsReport2.ReportItem[]))]
        public IHttpActionResult UBKReport([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.PoorBenefitsReport2.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.PrivilegePaymentReport1.ReportItem[]))]
        public IHttpActionResult FCReport([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.PrivilegePaymentReport1.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.AdditionalAksySocialBenefitsReport1.ReportItem[]))]
        public IHttpActionResult ASBAksyReport([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.AdditionalAksySocialBenefitsReport1.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.AdditionalSocialBenefitsReport1.ReportItem[]))]
        public IHttpActionResult ASBReport([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.AdditionalSocialBenefitsReport1.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.AdditionalSocialBenefitsReport1.ReportItem[]))]
        public IHttpActionResult SocialBenefitsReport([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.AdditionalSocialBenefitsReport1.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.LifetimeGrantReport1.ReportItem[]))]
        public IHttpActionResult LifetimeGrantReport([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.LifetimeGrantReport1.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.AdditionalLifetimeGrantReport1.ReportItem[]))]
        public IHttpActionResult AdditionalLifetimeGrantReport([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                
                return Ok(ReportExecutor.AdditionalLifetimeGrantReport1.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }




        /*ОТЧЕТНОСТЬ 2022***************************************************************************************************/
        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.MonthlyFC_1001.ReportItem[]))]
        public IHttpActionResult MonthlyFC_1001([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.MonthlyFC_1001.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.MonthlyDK_1002.ReportItem[]))]
        public IHttpActionResult MonthlyDK_1002([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.MonthlyDK_1002.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.FC_1003.ReportItem[]))]
        public IHttpActionResult FC_1003([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.FC_1003.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.FC_1004.ReportItem[]))]
        public IHttpActionResult FC_1004([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.FC_1004.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.FC_1005.ReportItem[]))]
        public IHttpActionResult FC_1005([FromUri]Guid userId, [FromUri]DateTime fd, [FromUri]DateTime ld)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.FC_1005.Execute(context, fd, ld));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.MonthlyGrant_1017.ReportItem[]))]
        public IHttpActionResult MonthlyGrant_1017([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.MonthlyGrant_1017.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Event2010_1018.ReportItem[]))]
        public IHttpActionResult Event2010_1018([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Event2010_1018.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1019.ReportItem[]))]
        public IHttpActionResult Report_1019([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_1019.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1020.ReportItem[]))]
        public IHttpActionResult Report_1020([FromUri]Guid userId, [FromUri]DateTime fd, [FromUri]DateTime ld)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_1020.Execute(context, fd, ld));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1021.ReportItem[]))]
        public IHttpActionResult Report_1021([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_1021.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.FC_1022.ReportItem[]))]
        public IHttpActionResult FC_1022([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.FC_1022.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }
               
        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1023.ReportItem[]))]
        public IHttpActionResult Report_1023([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_1023.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.FC_1024.ReportItem[]))]
        public IHttpActionResult FC_1024([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.FC_1024.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1027.ReportItem[]))]
        public IHttpActionResult Report_1027([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_1027.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1028.ReportItem[]))]
        public IHttpActionResult Report_1028([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_1028.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1029.ReportItem[]))]
        public IHttpActionResult Report_1029([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_1029.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1029a.ReportItem[]))]
        public IHttpActionResult Report_1029a([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_1029a.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1029b.ReportItem[]))]
        public IHttpActionResult Report_1029b([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_1029b.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1030.ReportItem[]))]
        public IHttpActionResult Report_1030([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_1030.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.MonthlySocialBenefits_1032.ReportItem[]))]
        public IHttpActionResult MonthlySocialBenefits_1032([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.MonthlySocialBenefits_1032.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1032A.ReportItem[]))]
        public IHttpActionResult Report_1032A([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_1032A.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1032DSO.ReportItem[]))]
        public IHttpActionResult Report_1032DSO([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_1032DSO.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1036.ReportItem[]))]
        public IHttpActionResult Report_1036([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_1036.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_1037.ReportItem[]))]
        public IHttpActionResult Report_1037([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_1037.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_2001.ReportItem[]))]
        public IHttpActionResult Report_2001([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_2001.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_2001A.ReportItem[]))]
        public IHttpActionResult Report_2001A([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_2001A.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_2001B.ReportItem[]))]
        public IHttpActionResult Report_2001B([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_2001B.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_2003.ReportItem[]))]
        public IHttpActionResult Report_2003([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_2003.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_2006.ReportItem[]))]
        public IHttpActionResult Report_2006([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_2006.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_2007.ReportItem[]))]
        public IHttpActionResult Report_2007([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_2007.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_3021.ReportItem[]))]
        public IHttpActionResult Report_3021([FromUri] Guid userId, [FromUri] DateTime fd, [FromUri] DateTime ld)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_3021.Execute(context, fd, ld));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.PatientCountOfMonth_6001.ReportItem[]))]
        public IHttpActionResult PatientCountOfMonth_6001([FromUri] Guid userId, [FromUri] DateTime fd, [FromUri] DateTime ld)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.PatientCountOfMonth_6001.Execute(context, fd, ld));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_6002.ReportItem[]))]
        public IHttpActionResult Report_6002([FromUri] Guid userId, [FromUri] DateTime fd, [FromUri] DateTime ld)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_6002.Execute(context, fd, ld));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_6003.ReportItem[]))]
        public IHttpActionResult Report_6003([FromUri] Guid userId, [FromUri] DateTime fd, [FromUri] DateTime ld)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_6003.Execute(context, fd, ld));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/

        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_F8SVOD.ReportF8SVODItem[]))]
        public IHttpActionResult Report_F8SVOD([FromUri] Guid userId, [FromUri] DateTime fd, [FromUri] DateTime ld)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.Report_F8SVOD.Execute(context, fd, ld));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.ReportForNBKR.ReportItem[]))]
        public IHttpActionResult ReportForNBKR([FromUri] Guid userId, [FromUri] int year, [FromUri] int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));

                return Ok(ReportExecutor.ReportForNBKR.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        
        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_Application5.ReportItem[]))]
        public IHttpActionResult Report_Application5([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_Application5.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        /*******************************************************************************************************************/
        [HttpGet]
        [ResponseType(typeof(ReportExecutor.Report_Application5a.ReportItem[]))]
        public IHttpActionResult Report_Application5a([FromUri]Guid userId, [FromUri]int year, [FromUri]int month)
        {
            try
            {
                var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, userId), new DataContext(null));
                return Ok(ReportExecutor.Report_Application5a.Execute(context, year, month));
            }
            catch (Exception e)
            {
                return BadRequest(e.GetBaseException().Message);
            }
        }

        
    }
}   