![Logo](../../favicon.ico)
# Puppy.Web.Models.Api
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- For Paging: use search keyword by `terms`, page size and page number by `skip` and `take`
- Response `Collection`, example response use in [Puppy.Logger](../../../Puppy.Logger/readme.md)
```csharp
/// <summary>
///     Create Content Result with response type is <see cref="PagedCollectionModel{LogEntity}" /> 
/// </summary>
/// <param name="urlHelper"></param>
/// <param name="skip">     </param>
/// <param name="take">     </param>
/// <param name="terms">    
///     Search for <see cref="LogEntity.Id" />, <see cref="LogEntity.Message" />,
///     <see cref="LogEntity.Level" />, <see cref="LogEntity.CreatedTime" /> (with string
///     format is <c> "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK" </c>, ex: "2017-08-24T00:56:29.6271125+07:00")
/// </param>
/// <returns></returns>
/// <remarks>
///     <para>
///         Logger write Log with **message queue** so when create a log it *near real-time log*
///     </para>
///     <para>
///         Base on Http Request will return <c> ContentType XML </c> when Request Header
///         Accept or ContentType is XML, else return <c> ContentType Json </c>
///     </para>
/// </remarks>
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
