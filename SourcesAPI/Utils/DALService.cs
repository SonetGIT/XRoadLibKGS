
using Intersoft.CISSA.DataAccessLayer.Model.Context;
using Intersoft.CISSA.DataAccessLayer.Model.Query.Builders;
using Intersoft.CISSA.DataAccessLayer.Model.Query.Sql;
using Intersoft.CISSA.DataAccessLayer.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SourcesAPI.Utils
{
    public static class DALService
    {
        //Defs
        static Guid appDefId = new Guid("{04D25808-6DE9-42F5-8855-6F68A94A224C}");
        public static Guid postOrderDefId = new Guid("{19EA8D75-2EE7-42CA-BE3B-D7E41F343DDD}");
        public static Guid bankOrderDefId = new Guid("{A7D9505F-F3BE-4ABC-95E7-129D907C0FD8}");
        static Guid notificationOfCompletionDefId = new Guid("{5C02BCC3-0D7D-4E1A-9127-B27A871100E8}");
        static Guid notificationOfPreCompletionDefId = new Guid("{C5626180-CA36-494F-BF56-E1FB4A205DC6}");
        static Guid notificationOfPassportDefId = new Guid("{7ACD2A1D-2D07-4E82-9111-87BC8E60D44E}");

        //Users
        static Guid systemUserGlobalId = new Guid("{D9326117-48D5-44F1-957C-2218320033BB}");
        static Guid dszUserId = new Guid("{64C178E7-3F82-4E44-A7A1-FECB5FE693FF}");

        //Statuses
        static Guid onPaymentStateTypeId = new Guid("{3DB4DB00-3A7F-4228-A9A3-A413B85C18B4}");
        static Guid approvedStateTypeId = new Guid("{66D7FA1C-77EF-470D-A70B-0D6E5E16D942}");
        static Guid completedStateTypeId = new Guid("{E5FC0675-5703-4CD3-BFB6-BC2ADAF02627}");//снятые по сроку

        //Типы документов для уведомления
        public static Guid assignmentDocumentEnumItemId = new Guid("{C3F726BE-B5E0-4309-B371-F3AB56828725}");//Назначение
        public static Guid postOrderDocumentEnumItemId = new Guid("{A9ACEB48-1568-49B6-89CC-B53B2D1A4F14}");//Почтовое поручение
        public static Guid bankOrderDocumentEnumItemId = new Guid("{FF4BA56B-3C54-422E-A009-F4CD7C3C6AB6}");//Банковское поручение

        public static List<NotificationForCompleteItem> GetExpiredAssignments(DateTime forDate, bool withTop10 = false)
        {
            var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, systemUserGlobalId), new DataContext(null));

            var list = new List<NotificationForCompleteItem>();
            var table = new System.Data.DataTable();
            var qb = new QueryBuilder(appDefId);
            qb.Where("&State").Eq(onPaymentStateTypeId).And("AssignToMax").Lt(forDate)/*.And("&OrgId").Eq(new Guid("{915D0BEB-53C1-440F-9CBA-00612369FEFC}"))*/;
            var query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
            query.AddAttribute("&Id");
            query.AddAttribute("&OrgId");
            query.AddAttribute("PaymentType");
            if (withTop10)
                query.TopNo = 10;
            using (var reader = new SqlQueryReader(context.DataContext, query))
            {
                reader.Open();
                reader.Fill(table);
                reader.Close();
            }

            var docRepo = context.Documents;

            foreach(System.Data.DataRow row in table.Rows)
            {
                var id = (Guid)row[0];
                if (row[1] is DBNull || row[2] is DBNull) continue;

                var orgId = (Guid)row[1];
                var paymentTypeId = (Guid)row[2];
                var item = GetItem(orgId, paymentTypeId, list, context);
                item.Docs.Add(id);
            }

            return list;
        }

        public static List<NotificationOfPassportExpiration> GetExpiredPassportAssignments(DateTime forDate, bool withTop10 = false)
        {
            var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, systemUserGlobalId), new DataContext(null));

            var list = new List<NotificationOfPassportExpiration>();
            var table = new System.Data.DataTable();
            var qb = new QueryBuilder(appDefId);
            qb.Where("&State").Eq(onPaymentStateTypeId).And("AssignToMax").Lt(forDate).And("Applicant").Include("PassportExpiryDate").Le(forDate.AddMonths(1)).End()/*.And("&OrgId").Eq(new Guid("{915D0BEB-53C1-440F-9CBA-00612369FEFC}"))*/;
            var query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
            query.AddAttribute("Applicant");
            query.AddAttribute("&OrgId");
            query.AddAttribute("PaymentType");
            if (withTop10)
                query.TopNo = 10;
            using (var reader = new SqlQueryReader(context.DataContext, query))
            {
                reader.Open();
                reader.Fill(table);
                reader.Close();
            }

            var docRepo = context.Documents;

            foreach (System.Data.DataRow row in table.Rows)
            {
                if (row[0] is DBNull || row[1] is DBNull || row[2] is DBNull) continue;

                var personId = (Guid)row[0];
                var orgId = (Guid)row[1];
                var paymentTypeId = (Guid)row[2];
                var item = new NotificationOfPassportExpiration
                {
                    OrgId = orgId,
                    ApplicantId = personId,
                    OrgName = context.Orgs.GetOrgName(orgId),
                    PaymentTypeId = paymentTypeId
                };

                list.Add(item);
            }

            return list;
        }

        public static List<NotificationForCompleteItem> GetExpiredOrders(DateTime forDate, Guid docDefId, bool withTop10 = false)
        {
            var context = new WorkflowContext(new WorkflowContextData(Guid.Empty, systemUserGlobalId), new DataContext(null));

            var list = new List<NotificationForCompleteItem>();
            var table = new System.Data.DataTable();
            var qb = new QueryBuilder(docDefId);
            qb.Where("&State").Eq(approvedStateTypeId).And("ExpiryDate").Lt(forDate)/*.And("&OrgId").Eq(new Guid("{915D0BEB-53C1-440F-9CBA-00612369FEFC}"))*/;
            var query = SqlQueryBuilder.Build(context.DataContext, qb.Def);
            var appSrc = query.JoinSource(query.Source, appDefId, SqlSourceJoinType.Inner, "Application");
            query.AddAttribute(query.Source, "&Id");
            query.AddAttribute(query.Source, "&OrgId");
            query.AddAttribute(appSrc, "PaymentType");
            if (withTop10)
                query.TopNo = 10;
            using (var reader = new SqlQueryReader(context.DataContext, query))
            {
                reader.Open();
                reader.Fill(table);
                reader.Close();
            }

            var docRepo = context.Documents;

            foreach (System.Data.DataRow row in table.Rows)
            {
                var id = (Guid)row[0];
                if (row[1] is DBNull || row[2] is DBNull) continue;

                var orgId = (Guid)row[1];
                var paymentTypeId = (Guid)row[2];
                var item = GetItem(orgId, paymentTypeId, list, context);
                item.Docs.Add(id);
            }

            return list;
        }

        public static void ApplyCompletionDocs(List<Guid> docIdList, WorkflowContext context)
        {
            var docRepo = context.Documents;
            foreach(var docId in docIdList)
            {
                docRepo.SetDocState(docId, completedStateTypeId);
            }
        }

        public static void CreateNotificationOfCompletion(DateTime forDate, NotificationForCompleteItem item, WorkflowContext context, Guid documentTypeId)
        {
            var docRepo = context.Documents;
            var notify = docRepo.New(notificationOfCompletionDefId);
            notify["Date"] = forDate;
            notify["OrgName"] = item.OrgName;
            notify["PaymentType"] = item.PaymentTypeId;
            notify["CompletedAmount"] = item.Amount;
            notify["SocialDocumentType"] = documentTypeId;
            docRepo.Save(notify);
        }
        public static void CreateNotificationOfPreCompletion(DateTime forDate, NotificationForCompleteItem item, WorkflowContext context, Guid documentTypeId)
        {
            var docRepo = context.Documents;
            var notify = docRepo.New(notificationOfPreCompletionDefId);
            notify["Date"] = forDate;
            notify["OrgName"] = item.OrgName;
            notify["PaymentType"] = item.PaymentTypeId;
            notify["CompletedAmount"] = item.Amount;
            notify["SocialDocumentType"] = documentTypeId;
            docRepo.Save(notify);
        }
        public static void CreateNotificationOfPassportExpiration(DateTime forDate, NotificationOfPassportExpiration item, WorkflowContext context, Guid documentTypeId)
        {
            var docRepo = context.Documents;
            var notify = docRepo.New(notificationOfPassportDefId);
            notify["Date"] = forDate;
            notify["OrgName"] = item.OrgName;
            notify["Person"] = item.ApplicantId;
            notify["PaymentType"] = item.PaymentTypeId;
            notify["SocialDocumentType"] = documentTypeId;
            docRepo.Save(notify);
        }
        public static WorkflowContext GetUserContext(Guid orgId)
        {
            return new WorkflowContext(new WorkflowContextData(Guid.Empty, GetUserIdByOrgId(orgId)), new DataContext(null));
        }
        public static List<UserOrganization> UserOrganizations { get; set; }
        public static Guid GetUserIdByOrgId(Guid orgId)
        {
            var orgs = new List<UserOrganization>();
            if (UserOrganizations == null || UserOrganizations.Count == 0)
            {
                foreach (var el in XDocument.Load(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/CISSAOrganizationUsers.xml")).Root.Elements())
                {
                    orgs.Add(new UserOrganization
                    {
                        OrgName = el.Element("name").Value,
                        OrgId = Guid.Parse(el.Element("Id").Value),
                        UserId = Guid.Parse(el.Element("SystemUserId").Value)
                    });
                }
            }
            var orgObj = orgs.FirstOrDefault(x => x.OrgId == orgId);
            if (orgObj != null) return orgObj.UserId;
            else throw new ApplicationException("Не могу определить пользователя РУСР. OrgId не содержится в существующем справочнике районов! Значение выбранного OrgId: " + orgId);
        }
        static NotificationForCompleteItem GetItem(Guid orgId, Guid paymentTypeId, List<NotificationForCompleteItem> items, WorkflowContext context)
        {
            var item = items.FirstOrDefault(x => x.OrgId == orgId && x.PaymentTypeId == paymentTypeId);
            if(item == null)
            {
                item = new NotificationForCompleteItem
                {
                    OrgId = orgId,
                    PaymentTypeId = paymentTypeId,
                    OrgName = context.Orgs.GetOrgName(orgId),
                    PaymentTypeName = context.Enums.GetValue(paymentTypeId).Value,
                    Docs = new List<Guid>()
                };
                items.Add(item);
            }
            return item;
        }
        public class NotificationForCompleteItem
        {
            public Guid OrgId { get; set; }
            public string OrgName { get; set; }
            public Guid PaymentTypeId { get; set; }
            public string PaymentTypeName { get; set; }
            public Guid ApplicantId { get; set; }
            public int Amount
            {
                get
                {
                    return Docs.Count;
                }
            }
            public List<Guid> Docs { get; set; } = new List<Guid>();
        }
        public class NotificationOfPassportExpiration
        {
            public Guid OrgId { get; set; }
            public string OrgName { get; set; }
            public Guid PaymentTypeId { get; set; }
            public Guid ApplicantId { get; set; }
        }
        public class UserOrganization
        {
            public string OrgName { get; set; }
            public Guid OrgId { get; set; }
            public Guid UserId { get; set; }
        }
    }
}