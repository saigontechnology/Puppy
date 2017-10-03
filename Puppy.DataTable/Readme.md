![Logo](favicon.ico)
# Puppy.DataTable Document
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- Configurable for DataTable from Server Side.

- Support Search: `Text`, `DateTime`, `Enum`, `Number`.
  > Support response and search Enum data as `Display Name` ?? `Description` ?? `Name`

- Support Custom and transform response data. Ex: Difference `DateTime` format for each endpoint.

- Support set global format for `DateTime` property.
  > If `RequestDateTimeFormatMode` is Auto, the string will auto parse to `DateTime` by any format, else will use global DateTimeFormat.

- Support Filter Type: `Select`, `Text`, `None`.

- Support submit `Additional Data` for each request, you can access all data including additional in server-side via `Data` property.

- Support Develop Mode: for debug purpose: `console.log` request, response and config of DataTable.

# How To Use
- Copy `Assets/jquery.dataTables.columnFilter.min.js` to your `Scripts` folder.

- Add jQuery DataTable and column filter to your master page.
```html
<!-- Jquery + DataTable -->
<script type="text/javascript" rc="//code.jquery.com/jquery-1.12.4.min.js"></script>
<script type="text/javascript" src="//cdn.datatables.net/1.10.0/js/jquery.dataTables.min.js"></script>

<!-- You can find resources in folder "Assets"-->
<script type="text/javascript" src="jquery.dataTables.columnFilter.min.js"></script>

<!-- ColVis -->
<script type="text/javascript" src="//cdn.datatables.net/colvis/1.1.2/js/dataTables.colVis.min.js"></script>
```

- Copy Foler `Assets/DataTable` to your master page folder, remember to `Right Click` > `Properties` > `Build Action`: `Content`, `Copy To Output Directory`: `Copy if newer`.

- Example to use `_DataTableHtml.cshtml and _DataTableScript.cshtml` is `Index.cshtml`.

- Add DataTable to Service Collection
```csharp
// [DataTable]
services.AddDataTable(ConfigurationRoot);
```

- Add `DataTable Model Binder Provider` to `Mvc Options`
```csharp
services
	.AddMvc(options =>
	{
		// [DataTable]
		options.AddDataTableModelBinderProvider();
	})
```

- Use DataTable for ApplicationBuidler
```csharp
// [DataTable]
app.UseDataTable();
```

- Add "DataTable" section in appsettings.json to config the DateTimeFormat
```javascript
// [Auto Reload]
"DataTable": {
// Response DateTime as string by format, default is "dd/MM/yyyy hh:mm tt".
// If RequestDateTimeFormatMode is Specific, every request will use the format to parse to DateTime.
"DateTimeFormat": "dd/MM/yyyy hh:mm tt",

// Control the way to parse string to DateTime every request.
// Value can be Auto or Specific, default is Auto.
"RequestDateTimeFormatMode": "Auto"
}
```

- In Business, return `DataTablesResponseDataModel` to Controller by `{IQueryable}.GetDataTableResponse({dataTableParamModel})`

- In Controller, response ActionResult by `{DataTablesResponseDataModel}.GetDataTableActionResult<T>()`

## Best Practive

### Server Side
```csharp
[HttpPost]
[Route("users")]
public DataTableActionResult<UserFacetRowViewModel> GetFacetedUsers([FromBody]DataTableParamModel dataTableParamModel)
{
    var query = FakeDatabase.Users.Select(
        user =>
            new UserFacetRowViewModel
            {
                Email = user.Email,
                Position = user.Position == null ? "" : user.Position.ToString(),
                Hired = user.Hired,
                IsAdmin = user.IsAdmin,
                Content = "https://randomuser.me/api/portraits/thumb/men/" + user.Id + ".jpg"
            });

    var response = query.GetDataTableResponse(dataTableParamModel);

    var result = response.GetDataTableActionResult<UserFacetRowViewModel>(
        row =>
            new
            {
                Content = "<div>" +
                            "  <div>Email: " + row.Email + (row.IsAdmin ? " (admin)" : "") +
                            "</div>" +
                            "  <div>Hired: " + row.Hired + "</div>" +
                            "  <img src='" + row.Content + "' />" +
                            "</div>"
            });

    return result;
}
```

### Frontend Side
- Please follow sample: 
	+ `Assets/Index.cshtml`
	+ `Assets/DataTable/_DataTableHtml.cshtml`
	+ `Assets/DataTable/_DataTableScript.cshtml`

### Sample Customising column rendering
```csharp
public class Message
{
    public DateTime CreatedDate{get;set;}
    public User User {get;set;} 
    public ICollection<Recipients> Recipients {get; set;}
    public string Text {get;set;}
}

public class MessageViewModel
{
    public DateTime CreatedDate { get; set; }
    public string User { get; set; } 
     // Dont want this as a column, we are keeping it here so we can use it in the transform 
    [DataTablesExclude]
    public User UserEntity {get;set;} 
    public string Text {get;set;}
}
```

### Specifying Initial Search Values
- Use can define data for `DataTableModel` in both `.cs` file and `.cshtml` file (via Razor code)
```csharp
model
    .FilterOn("Position", new { sSelector = "#custom-filter-placeholder-position" }, new { sSearch = "Tester" }).Select("Engineer", "Tester", "Manager")

    .FilterOn("Id", null, new { sSearch = "2~4", bEscapeRegex = false }).NumberRange()

    .FilterOn("IsAdmin", null, new { sSearch = "False" }).Select("True","False")

    .FilterOn("Salary", new { sSelector = "#custom-filter-placeholder-salary" }, new { sSearch = "1000~100000" }).NumberRange();

model.StateSave = false;

// ... other configs
```