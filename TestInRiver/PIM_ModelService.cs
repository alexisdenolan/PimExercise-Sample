using inRiver.Remoting;
using inRiver.Remoting.Objects;
using System.Globalization;
using System.IO;
using System.Linq;

namespace TestInRiver
{
    public class PIM_ModelService
    {
        public void Categories()
        {
            var cat = RemoteManager.ModelService.GetAllCategories();
            var catByEntity = RemoteManager.ModelService.GetCategoriesForEntityType("ProductChannelNodeId");
            var category1 = RemoteManager.ModelService.GetCategory("General");

            if (cat.Count <= 0)
            {
                var category = new Category();
                category.Id = "MyNewCategory";
                category.Index = 2;
                var name = new LocaleString(RemoteManager.UtilityService.GetAllLanguages());
                name[new CultureInfo("en")] = "My new category";
                name[new CultureInfo("sv")] = "Min nya kategori";
                category.Name = name;
                category = RemoteManager.ModelService.AddCategory(category);
            }
        }

        public void AddEntityType(string entityName = "MyNewEntityType1")
        {
            var entityType = new EntityType();
            entityType.Id = entityName;

            var ls = new LocaleString(RemoteManager.UtilityService.GetAllLanguages());
            ls[new CultureInfo("en")] = "My new EntityType";
            ls[new CultureInfo("sv")] = "Min nya entitetstyp";

            entityType.Name = ls;
            entityType = RemoteManager.ModelService.AddEntityType(entityType);

            if (entityType != null) AddFieldType(entityType.Id);
        }

        private void AddCvlValue(string cvlId, string newCVLValueKey, string newCVLValue)
        {
            CVL cvl = RemoteManager.ModelService.GetCVL(cvlId);
            CVLValue cvlVal = new CVLValue() { CVLId = cvlId, Key = newCVLValueKey };

            if (cvl.DataType == DataType.String)
            {
                cvlVal.Value = newCVLValue;
            }
            else if (cvl.DataType == DataType.LocaleString)
            {
                LocaleString locale = new LocaleString(RemoteManager.UtilityService.GetAllLanguages());
                CultureInfo cultureInfo = new CultureInfo("en");
                locale[cultureInfo] = newCVLValue;
                cvlVal.Value = locale;
            }

            RemoteManager.ModelService.AddCVLValue(cvlVal);
        }

        public void FieldSets()
        {
            var fieldTypes = RemoteManager.ModelService.GetFieldTypesForFieldSet("ItemHygiene");
            var fieldSets = RemoteManager.ModelService.GetFieldSetsForFieldType("ItemHeight");
        }

        public FieldType AddFieldType(string entityName)
        {
            var fieldtype = new FieldType();
            fieldtype.Id = "MyNewFieldtype";
            fieldtype.DataType = DataType.CVL;
            fieldtype.CVLId = "Manufacturer";
            fieldtype.CategoryId = "General";
            fieldtype.EntityTypeId = entityName;

            var name = new LocaleString(RemoteManager.UtilityService.GetAllLanguages());
            name[new CultureInfo("en")] = "My new FieldType";
            name[new CultureInfo("sv")] = "Min nya Fälttyp";

            fieldtype.Name = name;
            fieldtype.Index = 200;
            fieldtype.Multivalue = true;

            return RemoteManager.ModelService.AddFieldType(fieldtype);
        }

        public void AddProductEntity()
        {
            Entity productEntity = Entity.CreateEntity(RemoteManager.ModelService.GetEntityType("Product"));


            Field prodId = productEntity.Fields.Find(f => f.FieldType.Id == "ProductId");
            if (prodId != null) prodId.Data = "sample prod id2";
            Field prodName = productEntity.Fields.Find(f => f.FieldType.Id == "ProductName");
            if (prodName != null) prodName.Data = "sample prod name2";

            Field prodDisplayName = productEntity.Fields.Find(f => f.FieldType.Id == "ProductDisplayName");
            if (prodDisplayName != null)
            {
                var localStringValues = new LocaleString(RemoteManager.UtilityService.GetAllLanguages());
                localStringValues[new CultureInfo("en")] = "Hello";
                localStringValues[new CultureInfo("sv")] = "Hallå";
                prodDisplayName.Data = localStringValues;
            }

            RemoteManager.DataService.AddEntity(productEntity);
        }

        public void AddResourceEntity()
        {
            EntityType resourceEntity = RemoteManager.ModelService.GetEntityType("Resource");
            Entity resource = Entity.CreateEntity(resourceEntity);
            var fieldTypes = resourceEntity.FieldTypes;

            fieldTypes.ForEach(f =>
            {
                resource.GetField(f.Id).Data = "hello";
                var res = resource.GetField(f.Id).Data;

                if (f.DataType == DataType.LocaleString)
                {
                    var localStringValues = new LocaleString(RemoteManager.UtilityService.GetAllLanguages());
                    resource.GetField(f.Id).Data = localStringValues;
                    var localeString = resource.GetField(f.Id).Data;
                }

                if (f.DataType == DataType.CVL)
                {
                    CVL cvlObj = new CVL();
                    var cvl1 = RemoteManager.ModelService.GetCVL(f.CVLId);
                    string dataType = RemoteManager.ModelService.GetCVL(f.CVLId).DataType;
                    cvlObj.DataType = dataType;
                    cvlObj.Id = dataType == DataType.LocaleString
                        ? "NewCVLAsLocaleStringId"
                        : "NewCVLAsStringId";
                    resource.GetField(f.Id).Data = cvlObj;

                    var cvl = resource.GetField(f.Id).Data;

                }

                if (f.Equals(fieldTypes.Last()))
                {
                    RemoteManager.DataService.AddEntity(resource);
                }
            });
        }

        public void ExportImportXMLEntity()
        {
            var export = RemoteManager.ModelService.ExportModelAsXmlString(true);
            using (var sw = new StreamWriter(@"c:\temp\model.xml"))
            {
                sw.Write(export);
            }

            using (var sr = new StreamReader(@"c:\temp\model.xml"))
            {
                string xml = sr.ReadToEnd();
                bool ok = RemoteManager.ModelService.ImportModelFromXmlString(xml);
            }

        }
    }
}
