namespace Puppy.Elastic.Model.SearchModel
{
    public interface IQuery
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}