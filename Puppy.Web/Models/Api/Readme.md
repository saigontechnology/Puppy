![Logo](../../favicon.ico)
# Puppy.Web.Models.Api
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- For Paging: use search keyword by `terms`, page size and page number by `skip` and `take`
- Response `Collection`, example response use in [Puppy.Logger](../../../Puppy.Logger/readme.md)
```csharp
public const string LogsEndpointPattern = "logs/{skip:int}/{take:int}";

/// <summary>
///     Logs 
/// </summary>
/// <returns></returns>
[ServiceFilter(typeof(ViewLogViaUrlAccessFilter))]
[HttpGet]
[Route(LogsEndpointPattern)]
public IActionResult GetLogs([FromRoute]int skip, [FromRoute]int take, [FromQuery]string terms)
{
    Expression<Func<LogEntity, bool>> predicate = null;

    if (!string.IsNullOrWhiteSpace(terms))
    {
        predicate = x => x.Message.Contains(terms);
    }

    var logs = Log.Get(out long total, predicate: predicate, orders: x => x.CreatedTime, isOrderByDescending: true, skip: skip, take: take);

    if (total <= 0)
    {
        // Return 204 for No Data Case
        return NoContent();
    }

    var placeholderLinkView = PlaceholderLinkViewModel.ToCollection(LogsEndpointPattern, HttpMethod.Get.Method, new { skip, take, terms });
    var collectionFactoryViewModel = new PagedCollectionFactoryViewModel<LogEntity>(placeholderLinkView, LogsEndpointPattern);
    var collectionViewModel = collectionFactoryViewModel.CreateFrom(logs, skip, take, total);
    return Ok(collectionViewModel);
}
```    
