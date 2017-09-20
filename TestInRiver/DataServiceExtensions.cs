using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using inRiver.Remoting;
using inRiver.Remoting.Objects;
using inRiver.Remoting.Query;

namespace TestInRiver
{
    public static class DataServiceExtensions
    {
        public static IEnumerable<Entity> FindEntitiesByFieldValue(this DataService dataService, string fieldTypeId, string value, LoadLevel level = LoadLevel.DataAndLinks)
        {
            var query = new Query();
            var criteria = new Criteria
            {
                FieldTypeId = fieldTypeId,
                Operator = Operator.Equal,
                Value = value
            };

            query.Criteria = new List<Criteria> { criteria };

            return RemoteManager.DataService.Search(query, level);
        }

        public static Link AddLinkBetweenEntities(this DataService dataService, Entity source, Entity target)
        {
            var linkTypes = RemoteManager.ModelService.GetLinkTypesForEntityType(source.EntityType.Id);
            var linkType = linkTypes.Find(l => l.SourceEntityTypeId == source.EntityType.Id && l.TargetEntityTypeId == target.EntityType.Id);

            var link = new Link
            {
                LinkType = linkType,
                Source = source,
                Target = target
            };

            return RemoteManager.DataService.AddLinkLast(link);
        }

        public static bool AddLinksBetweenEntities(this DataService dataService, Entity source, List<Entity> targets)
        {
            if (targets == null || !targets.Any()) return false;

            var linkTypes = RemoteManager.ModelService.GetLinkTypesForEntityType(source.EntityType.Id);
            var links = new List<Link>();

            foreach (var target in targets)
            {
                var linkType = linkTypes.Find(l => l.SourceEntityTypeId == source.EntityType.Id && l.TargetEntityTypeId == target.EntityType.Id);

                var link = new Link
                {
                    LinkType = linkType,
                    Source = source,
                    Target = target
                };

                links.Add(link);
            }

            return RemoteManager.DataService.AddLinks(links);
        }
    }
}
