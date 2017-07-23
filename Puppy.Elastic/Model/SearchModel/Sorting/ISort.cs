namespace Puppy.Elastic.Model.SearchModel.Sorting
{
    public interface ISort
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}