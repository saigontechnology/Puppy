namespace Puppy.Elastic.Model.SearchModel
{
    public interface IFilter
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}