using inRiver.Remoting;
using inRiver.Remoting.Objects;
using inRiver.Remoting.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TestInRiver
{
    public class PIM_DataService
    {


        public void Links()
        {
            //Entity product = RemoteManager.DataService.FindEntitiesByFieldValue("ProductId", "550060-grouped").FirstOrDefault();
            //Entity item = RemoteManager.DataService.FindEntitiesByFieldValue("ItemId", "550108").FirstOrDefault();

            ////LINKS
            //// Create links between our new entities
            //Link productToItemLink = new Link();
            //// Find a link type to use for a "Product to Item" link
            //// The link source is our product and the target is our item, so try to find a link type 
            //// with source entity type = "Product" and target entity type = "Item"
            //List<LinkType> linkTypes = RemoteManager.ModelService.GetLinkTypesForEntityType(product?.EntityType.Id);
            //LinkType productToItemLinkType = linkTypes.Find(l => l.TargetEntityTypeId == item?.EntityType.Id);
            //productToItemLink.LinkType = productToItemLinkType;
            //productToItemLink.Source = product;
            //productToItemLink.Target = item;
            //RemoteManager.DataService.AddLinkLast(productToItemLink);

            Entity itemParent = RemoteManager.DataService.FindEntitiesByFieldValue("ItemId", "550076").FirstOrDefault();
            Entity itemChild = RemoteManager.DataService.FindEntitiesByFieldValue("ItemId", "550078").FirstOrDefault();

            var result = RemoteManager.DataService.GetInboundLinksForEntity(itemParent.Id);
            //LINKS
            // Create links between our new entities
            Link itemToItemLink = new Link();
            // Find a link type to use for a "Product to Item" link
            // The link source is our product and the target is our item, so try to find a link type 
            // with source entity type = "Product" and target entity type = "Item"
            List<LinkType> linkTypes1 = RemoteManager.ModelService.GetLinkTypesForEntityType(itemParent?.EntityType.Id);
            LinkType itemToItemLinkType = linkTypes1.Find(l => l.TargetEntityTypeId == itemChild?.EntityType.Id);
            itemToItemLink.LinkType = itemToItemLinkType;
            itemToItemLink.Source = itemParent;
            itemToItemLink.Target = itemChild;
            RemoteManager.DataService.AddLinkLast(itemToItemLink);


        }

        /// <summary>
        /// SEARCH FOR LINKS IN PRODUCTS, ITEMS, etc.
        /// </summary>
        public void Search()
        {
            //PRODUCT/ITEMS WITH LINKS
            LinkQuery linqQuery = new LinkQuery();
            linqQuery.LinkTypeId = "ProductItem";
            linqQuery.SourceEntityTypeId = "Product";   //OUTBOUND
            linqQuery.TargetEntityTypeId = "Item";      //INBOUND
            // Gives Items as result
            linqQuery.Direction = LinkDirection.InBound;
            List<Entity> entities = RemoteManager.DataService.LinkSearch(linqQuery, LoadLevel.Shallow);
            // Gives Products as result
            linqQuery.Direction = LinkDirection.OutBound;
            List<Entity> entities2 = RemoteManager.DataService.LinkSearch(linqQuery, LoadLevel.Shallow);

            //PRODUCTS WITH NO LINKS
            LinkQuery lq = new LinkQuery();
            lq.LinkTypeId = "ProductItem";
            lq.SourceEntityTypeId = "Product";
            //lq.Direction = LinkDirection.InBound;
            var resourceWithNoProducts = RemoteManager.DataService.LinkSearch(lq, LoadLevel.Shallow);

            //ITEMS WITH NO LINKS
            LinkQuery lq1 = new LinkQuery();
            lq1.LinkTypeId = "ProductItem";
            lq1.TargetEntityTypeId = "Item";
            //lq1.Direction = LinkDirection.InBound;
            var resourceWithNoItems = RemoteManager.DataService.LinkSearch(lq1, LoadLevel.Shallow);

            //ITERATE ITEMS
            foreach (var resource in resourceWithNoItems)
            {
                var resourceFilename = RemoteManager.DataService.GetFieldValue(resource.Id, "ItemName");
                var linkType = RemoteManager.ModelService.GetLinkType("ProductItem");
                var unique = RemoteManager.DataService.GetEntityByUniqueValue("ItemId", "550060", LoadLevel.Shallow);
            }
        }

        public void Specification()
        {
            var specFieldType = new SpecificationFieldType();
            specFieldType.Id = Guid.NewGuid().ToString().Replace("-", ""); //Example of auto generated Id
            specFieldType.EntityId = 3974; //SPECIFICATION ENTITY
            specFieldType.Index = 1;
            specFieldType.DataType = DataType.String;
            //specFieldType.CVLId = "Color";
            //specFieldType.Multivalue = true;
            specFieldType.Name = new LocaleString(RemoteManager.UtilityService.GetAllLanguages());
            specFieldType.Name[new CultureInfo("en")] = "Main Colors";
            specFieldType.Mandatory = false;
            specFieldType.CategoryId = "MyNewCategory";
            RemoteManager.DataService.AddSpecificationFieldType(specFieldType);

            var f = new SpecificationField();
            f.EntityId = 3932; //ENTITY TO LINK FOR (e.g. Product, Items, etc)
            f.Data = "My value";
            f.SpecificationFieldType = RemoteManager.DataService.GetSpecificationFieldType(specFieldType.Id); //GUID
            RemoteManager.DataService.AddSpecificationField(f);

            var category = new Category();
            category.Id = "MyNewCategory";
            category.Index = 2;
            var name = new LocaleString(RemoteManager.UtilityService.GetAllLanguages());
            name[new CultureInfo("en")] = "My new category";
            name[new CultureInfo("sv")] = "Min nya kategori";
            category.Name = name;
            RemoteManager.DataService.AddSpecificationCategory(category);

            //Enable/disable a current template (if disable it will not visible to specification)
            RemoteManager.DataService.EnableSpecificationTemplateFieldType(false, 3974, "7710e7b5445b4dd89ca008e6d442720c");
        }


        public void ResourcesLink()
        {
            LinkQuery lq = new LinkQuery()
            {
                Direction = LinkDirection.OutBound,
                LinkTypeId = "ItemResource",
                SourceEntityTypeId = "Item",
                TargetEntityTypeId = "Resource"
            };

            var searchLinkQuery = RemoteManager.DataService.LinkSearch(lq, LoadLevel.Shallow);


            if (searchLinkQuery.Count <= 0)
                AddLinkToResource();

            foreach (var item in searchLinkQuery)
            {
                var itemData = RemoteManager.DataService.GetEntityByUniqueValue("ItemId", item.DisplayName.Data.ToString(), LoadLevel.DataAndLinks);
                var resources = itemData.OutboundLinks.Where(c => c.LinkType.Id.Equals("ItemResource")).Select(c => c.Target).ToList();

            }

        }

        public void SetEntityTypeMainPicture()
        {
            Entity item = RemoteManager.DataService.FindEntitiesByFieldValue("ItemId", "550108").FirstOrDefault();

            RemoteManager.DataService.SetMainPicture(item.Id, 3977);
        }

        public void AddLinkToResource()
        {
            Entity resource = RemoteManager.DataService.FindEntitiesByFieldValue("ResourceName", "fb.png").FirstOrDefault();
            Entity item = RemoteManager.DataService.FindEntitiesByFieldValue("ItemId", "550076").FirstOrDefault();

            //LINKS
            // Create links between our new entities
            Link itemToItemLink = new Link();
            List<LinkType> linkTypes = RemoteManager.ModelService.GetLinkTypesForEntityType(item?.EntityType.Id);
            LinkType itemToResourceLinkType = linkTypes.Find(l => l.TargetEntityTypeId == resource?.EntityType.Id);
            itemToItemLink.LinkType = itemToResourceLinkType;
            itemToItemLink.Source = item;
            itemToItemLink.Target = resource;
            RemoteManager.DataService.AddLinkLast(itemToItemLink);
        }

        public void GetChannel()
        {
            var entities = RemoteManager.DataService.GetAllEntitiesForEntityType("Channel", LoadLevel.DataAndLinks);
            var entity = entities.Where(x => x.DisplayName.Data.ToString() == "Ecommerce").FirstOrDefault();

            if (entity != null)
            {
                var entityTarget = RemoteManager.DataService.GetOutboundLinksForEntity(entity.Id).FirstOrDefault().Target;
                var channelService = RemoteManager.ChannelService.GetEntitiesForNode(entity.Id, entityTarget.Id);

                foreach (var item in channelService)
                {
                    string data = item.DisplayName.Data.ToString();
                    UpdateProductFieldType(item, data);
                }
            }

        }


        private void UpdateProductFieldType(Entity productEntity, string data)
        {
            var entityType = RemoteManager.ModelService.GetEntityType("Product");
            var entity = Entity.CreateEntity(entityType);
            var product = RemoteManager.DataService.FindEntitiesByFieldValue("ProductName", data).FirstOrDefault();

            if (product != null)
            {
                product.GetField("ProductName").Data = "P" + data;
                RemoteManager.DataService.UpdateEntity(product);
            }



        }
    }
}
