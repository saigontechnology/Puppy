using System.Collections.Generic;

namespace Puppy.Web.Models.Api
{
    public class CollectionViewModel<T> : ResourceViewModel
    {
        public ICollection<T> Items { get; set; }
    }
}