namespace TopCore.Framework.Search.Elastic.Model.GeoModel
{
    public interface IGeoType
    {
        string Type { get; set; }

        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}