using System.Collections.Generic;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
    //"aliases" : {
    //  "april_2014" : {},
    //  "year_2014" : {}
    //},
    public class IndexAliases
    {
        public IndexAliases()
        {
            Aliases = new List<IndexAlias>();
        }

        public List<IndexAlias> Aliases { get; set; }

        //"aliases" : {
        //  "april_2014" : {},
        //  "year_2014" : {}
        //},
        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("aliases");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            foreach (var alias in Aliases)
                alias.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}