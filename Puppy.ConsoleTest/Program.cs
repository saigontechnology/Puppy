using Puppy.Web.HtmlUtils;

namespace Puppy.ConsoleTest
{
    internal class Program
    {
        private static void Main()
        {
            HtmlHelper.ToPdfFromHtml("<html><body>Top Nguyen</body></html>", @"E:\Top Nguyen.pdf");
        }
    }
}