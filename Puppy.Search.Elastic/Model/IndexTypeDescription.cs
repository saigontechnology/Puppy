namespace Puppy.Search.Elastic.Model
{
    public class IndexTypeDescription
    {
        public IndexTypeDescription(string index, string indexType)
        {
            Index = index;
            IndexType = indexType;
        }

        public string Index { get; }

        public string IndexType { get; }
    }
}