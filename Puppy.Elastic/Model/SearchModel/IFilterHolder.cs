namespace Puppy.Elastic.Model.SearchModel
{
    public interface IFilterHolder
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}