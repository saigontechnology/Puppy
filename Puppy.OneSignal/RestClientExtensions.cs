#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RestClientExtensions.cs </Name>
//         <Created> 30/05/2017 4:58:39 PM </Created>
//         <Key> e4a81464-d951-4757-a95c-aaf8c8946916 </Key>
//     </File>
//     <Summary>
//         RestClientExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using RestSharp;
using System.Threading.Tasks;

namespace Puppy.OneSignal
{
    public static class RestClientExtensions
    {
        public static Task<IRestResponse<T>> ExecuteAsync<T>(this IRestClient client, IRestRequest request) where T : new()
        {
            TaskCompletionSource<IRestResponse<T>> taskCompletion = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(request, r => taskCompletion.SetResult(r));
            return taskCompletion.Task;
        }

        public static Task<IRestResponse> ExecuteAsync(this IRestClient client, IRestRequest request)
        {
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(request, r => taskCompletion.SetResult(r));
            return taskCompletion.Task;
        }
    }
}