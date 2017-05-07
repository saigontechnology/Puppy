namespace TopCore.Framework.Search.Elastic.Model.SearchModel
{
	public interface IAggs
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}