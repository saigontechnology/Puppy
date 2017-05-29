namespace Puppy.Search.Elastic.Model.SearchModel.Sorting
{
    public interface ISort
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}