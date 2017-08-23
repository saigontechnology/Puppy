namespace Puppy.Web.Models.Api
{
    public class PagedCollectionViewModel<T> : CollectionViewModel<T>
    {
        public string HrefPattern { get; set; }

        public long Total { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public ILinkViewModel First { get; set; }

        public ILinkViewModel Previous { get; set; }

        public ILinkViewModel Next { get; set; }

        public ILinkViewModel Last { get; set; }
    }
}