![Logo](../../favicon.ico)
# Puppy.Web.Models.Api
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- For Paging: use search keyword by `terms`, page size and page number by `skip` and `take`
- Response `Collection`, example response use in [Puppy.Logger](../../../Puppy.Logger/readme.md)
- Or use can create object `PagedCollectionResultModel{T}` then in your controller use extensions by method `UrlHelperExtensions.GeneratePagedCollectionResult`to generate `Paged Collection Result`
```csharp
public static ContentResult GetLogsContentResult(IUrlHelper urlHelper, int skip, int take, string terms)
{
    Expression<Func<LogEntity, bool>> predicate = null;

    var termsNormalize = StringHelper.Normalize(terms);

    if (!string.IsNullOrWhiteSpace(termsNormalize))
    {
        predicate = x => x.Id.ToUpperInvariant().Contains(termsNormalize)
        || x.Message.ToUpperInvariant().Contains(termsNormalize)
        || x.Level.ToString().ToUpperInvariant().Contains(termsNormalize)
        || x.CreatedTime.ToString(Core.Constant.DateTimeOffSetFormat).Contains(termsNormalize);
    }

    var logs = Get(out long total, predicate: predicate, orders: x => x.CreatedTime, isOrderByDescending: true, skip: skip, take: take);

    ContentResult contentResult;

    if (total <= 0)
    {
        // Return 204 for No Data Case
        contentResult = new ContentResult
        {
            ContentType =
            (urlHelper.ActionContext.HttpContext.Request.Headers[HeaderKey.Accept] == ContentType.Xml ||
                urlHelper.ActionContext.HttpContext.Request.Headers[HeaderKey.ContentType] == ContentType.Xml)
                ? ContentType.Xml
                : ContentType.Json,
            StatusCode = (int)HttpStatusCode.NoContent,
            Content = null
        };

        return contentResult;
    }

    var collectionModel = new PagedCollectionFactoryModel<LogEntity>(urlHelper, skip, take, terms, total, logs, HttpMethod.Get.Method).Generate();

    if (urlHelper.ActionContext.HttpContext.Request.Headers[HeaderKey.Accept] == ContentType.Xml ||
        urlHelper.ActionContext.HttpContext.Request.Headers[HeaderKey.ContentType] == ContentType.Xml)
    {
        contentResult = new ContentResult
        {
            ContentType = ContentType.Xml,
            StatusCode = (int)HttpStatusCode.OK,
            Content = XmlHelper.ToXmlStringViaJson(collectionModel)
        };
    }
    else
    {
        contentResult = new ContentResult
        {
            ContentType = ContentType.Json,
            StatusCode = (int)HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(collectionModel, Core.Constant.JsonSerializerSettings)
        };
    }

    return contentResult;
}
```    
