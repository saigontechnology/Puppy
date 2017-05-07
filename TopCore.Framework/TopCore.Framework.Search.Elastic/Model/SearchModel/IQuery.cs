namespace TopCore.Framework.Search.Elastic.Model.SearchModel
{
	public interface IQuery
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}