namespace Puppy.Elastic.Model.SearchModel
{
    public interface IQueryHolder
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}