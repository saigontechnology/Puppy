#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RestClientExtensions.cs </Name>
//         <Created> 23/07/17 4:05:27 PM </Created>
//         <Key> 09abbb85-f442-4b3e-85e1-2aa5a53ca2df </Key>
//     </File>
//     <Summary>
//         RestClientExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Threading.Tasks;
using RestSharp;

namespace Puppy.RestSharp
{
    public static class RestClientExtensions
    {
        public static Task<IRestResponse<T>> ExecuteAsync<T>(this IRestClient client, IRestRequest request)
            where T : new()
        {
            var taskCompletion = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, r => taskCompletion.SetResult(r));
            return taskCompletion.Task;
        }

        public static Task<IRestResponse> ExecuteAsync(this IRestClient client, IRestRequest request)
        {
            var taskCompletion = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, r => taskCompletion.SetResult(r));
            return taskCompletion.Task;
        }
    }
}