using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvRollupNN
{
    public class App
    {
        private IOrganizationService svc;

        public App(IOrganizationService svc)
        {
            this.svc = svc;
            //this is a test
        }

        public void Run(Guid id)
        {
            var linked = getNN(svc, "cobalt_meeting", "dci_usergroup", "dci_dci_meeting_dci_usergroup", id);
            var names = linked.Select(e => e.GetAttributeValue<AliasedValue>("dci_usergroup2.dci_name").Value as string).ToList();
        }

        public void Run(EntityReference targetRef)
        {
            var linked = getNN(svc, "cobalt_meeting", "dci_usergroup", "dci_dci_meeting_dci_usergroup", targetRef.Id);
            var names = linked.Select(e => e.GetAttributeValue<AliasedValue>("dci_usergroup2.dci_name").Value as string).ToList();
            var target = new Entity(targetRef.LogicalName, targetRef.Id);
            target["cobalt_summary"] = string.Join(", ", names.ToArray());
            svc.Update(target);
        }

        public void Run(AssociateRequest req)
        {
            var relationship = req.Relationship;

            //"short circuit" conditions
            if (relationship == null
            || relationship.SchemaName.ToLower() != "dci_dci_meeting_dci_usergroup"
            || req.Target == null
            || req.RelatedEntities == null
            || req.RelatedEntities.Count == 0)
            {
                return;
            }

            var targetRef = req.Target;
            var relatedEntities = req.RelatedEntities;
            var relatedEntity = relatedEntities[0];

            var target = new Entity(targetRef.LogicalName, targetRef.Id);

            //target["cobalt_summary"] = context.MessageName;
            target["cobalt_summary"] = relatedEntity.Id.ToString();
            svc.Update(target);

            var linked = getNN(svc, "cobalt_meeting", "dci_usergroup", "dci_dci_meeting_dci_usergroup", target.Id);
        }

        private class QueryDef
        {
            public string Entity { get; private set; }
            public ColumnSet Columns { get; private set; }

            public QueryDef(string entity, ColumnSet cols)
            {

            } 
        }

        //private QueryExpression getQueryNN()
        private List<Entity> getNN(IOrganizationService svc, string parent, string child, string intersect, Guid parentId)
        {
            var parentIdField = $"{parent}id";
            var childIdField = $"{child}id";
            var query = new QueryExpression(parent);
            query.ColumnSet = new ColumnSet("cobalt_name", "cobalt_meetingid");
            var linkEntity1 = new LinkEntity(parent, intersect, parentIdField, parentIdField, JoinOperator.Inner);

            var linkEntity2 = new LinkEntity(intersect, child, childIdField, childIdField, JoinOperator.Inner);
            linkEntity2.Columns = new ColumnSet("dci_name", "dci_usergroupid");
            linkEntity1.LinkEntities.Add(linkEntity2);
            query.LinkEntities.Add(linkEntity1);

            query.Criteria = new FilterExpression
            {
                FilterOperator = LogicalOperator.And,
                Conditions =
                {
                    new ConditionExpression
                    {
                        AttributeName = parentIdField,
                        Operator = ConditionOperator.Equal,
                        Values = { parentId }
                    }
                }
            };

            return svc.RetrieveMultiple(query).Entities.ToList();
        }
    }
}
