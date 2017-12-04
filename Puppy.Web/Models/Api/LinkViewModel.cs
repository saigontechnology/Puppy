using System;
using System.Runtime.Serialization;

namespace Puppy.Web.Models.Api
{
    [Serializable]
    [KnownType(typeof(LinkViewModel))]
    public class LinkViewModel : ILinkViewModel
    {
        public string Href { get; set; }

        public string Method { get; set; }
    }
}