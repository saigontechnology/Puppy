namespace TopCore.Framework.Search.Elastic.Model.SearchModel
{
    public interface IFilter
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}