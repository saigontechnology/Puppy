namespace Puppy.Search.Elastic.Model.SearchModel
{
    public interface IFilterHolder
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}