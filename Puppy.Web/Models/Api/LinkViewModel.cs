namespace Puppy.Web.Models.Api
{
    public class LinkViewModel : ILinkViewModel
    {
        public string Href { get; set; }

        public string[] Relations { get; set; }

        public string Method { get; set; }
    }
}