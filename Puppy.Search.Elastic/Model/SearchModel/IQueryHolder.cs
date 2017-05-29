namespace Puppy.Search.Elastic.Model.SearchModel
{
    public interface IQueryHolder
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}