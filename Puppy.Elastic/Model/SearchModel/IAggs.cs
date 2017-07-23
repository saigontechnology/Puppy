namespace Puppy.Elastic.Model.SearchModel
{
    public interface IAggs
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}