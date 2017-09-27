![Logo](favicon.ico)
# Puppy.DataTable Document
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- Support response and request/search Enum as Display Name ?? Description ?? Name
- Support Search Number Range
- Support Search Date Time Range
- Support Filter Type: Select, Text, None
- Support Additional Data
- Support Develop Mode: for debug purpose.

# How To Use

- Add `DataTable Model Binder Provider` to `Mvc Options`

```csharp
  services
        .AddMvc(options =>
        {
            options.AddDataTableModelBinderProvider();
        })
```

- In Business, return `DataTablesResponseDataModel` to Controller by `{IQueryable}.GetDataTableResponse({dataTableParamModel})`
- In Controller, response ActionResult by `{DataTablesResponseDataModel}.GetDataTableActionResult<T>()`
- The below is sample code
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

- Then initial datatable in View by Model
- Main.cshtml

```html
@using Monkey.Controllers.Api
@using Puppy.DataTable
@using Puppy.DataTable.Models.Config.Column
@using Constants = Monkey.Constants
@{
    ViewData[Constants.ViewDataKey.Title] = "Portal";
}

<div class="page-header">
    <h1 class="page-title">DataTables</h1>
</div>
<div class="page-content">
    <div class="panel">
        <div class="panel-body">

            @{
                var model = Html.DataTableModel(Guid.NewGuid().ToString("N"), (TestApiController controller) => controller.GetFacetedUsers(null));
                model.IsDevelopMode = false;
                model.IsUseColumnFilter = true;

                model.Columns.Add(new ColumnModel("Action", typeof(string))
                {
                    DisplayName = "Action Col",
                    IsSearchable = false,
                    IsSortable = false,
                    MRenderFunction = "actionColRender"
                });

                model.BeforeSendFunctionName = "beforeSendHandle";
            }

            @await Html.PartialAsync("/Areas/Portal/Views/Shared/_DataTable.cshtml", model).ConfigureAwait(true)

            <script>
                function beforeSendHandle(data) {
                    data.push({
                        name: "newData",
                        value: "test modify data before send"
                    });

                    console.log("before send handle: ", data);
                }
                function actionColRender(data, type, row) {
                    return "<button class='btn btn-primary'>" + row[2] + "</button>";
                }
            </script>
        </div>
    </div>
</div>
```

- _DataTable.cshtml

```html
@using Newtonsoft.Json.Linq
@using Puppy.DataTable.Constants
@using Puppy.DataTable.Models.Config
@using Puppy.DataTable.Utils.Extensions
@model DataTableModel

<table id="@Model.Id" class="@(Model.TableClass ?? string.Empty)">
    <thead>
        @if (Model.IsUseColumnFilter)
        {
            <tr>
                @foreach (var column in Model.Columns)
                {
                    <th>@column.DisplayName</th>
                }
            </tr>
        }
        @if (!Model.IsHideHeader)
        {
            <tr>
                @foreach (var column in Model.Columns)
                {
                    <th class="@column.CssClassHeader">@column.DisplayName</th>
                }
            </tr>
        }
    </thead>
    <tbody>
        <tr>
            <td colspan="@Model.Columns.Count" class="dataTables_empty">
                Loading...
            </td>
        </tr>
    </tbody>
</table>

<script type="text/javascript">
    (function initDataTable() {
        if (!window.jQuery || !$.fn.DataTable) {
            setTimeout(initDataTable, 100);
            return;
        }
        var $table = $('#@Model.Id');
        @{
            var options = new JObject
            {
                [PropertyConst.Sorting] = new JRaw(Model.GetColumnSortingString()),
                [PropertyConst.IsProcessing] = true,
                [PropertyConst.IsStateSave] = Model.IsStateSave,
                [PropertyConst.StateDuration] = -1,
                [PropertyConst.IsServerSide] = true,
                [PropertyConst.IsFilter] = Model.IsShowGlobalSearchInput,
                [PropertyConst.Dom] = Model.Dom,
                [PropertyConst.IsResponsive] = Model.IsResponsive,
                [PropertyConst.IsAutoWidth] = Model.IsAutoWidthColumn,
                [PropertyConst.AjaxSource] = Model.AjaxUrl,
                [PropertyConst.ColumnDefine] = new JRaw(Model.GetColumnsJsonString()),
                [PropertyConst.SearchCols] = Model.GetSearchColumns(),
                [PropertyConst.LengthMenuDefine] = Model.LengthMenu != null ? new JRaw(Model.LengthMenu) : new JRaw(string.Empty),
                [PropertyConst.Language] = new JObject
                {
                    [PropertyConst.SearchSelector] = "_INPUT_",
                    [PropertyConst.LengthMenuSelector] = "_MENU_",
                    [PropertyConst.SearchPlaceholder] = "Search...",
                }
            };

            // Default Size
            if (Model.PageSize.HasValue)
            {
                options[PropertyConst.DisplayLength] = Model.PageSize;
            }

            // Language Code
            if (!string.IsNullOrWhiteSpace(Model.LanguageCode))
            {
                options[PropertyConst.LanguageCode] = new JRaw(Model.LanguageCode);
            }

            // Draw Call back function
            if (!string.IsNullOrWhiteSpace(Model.DrawCallbackFunctionName))
            {
                options[PropertyConst.FnDrawCallback] = new JRaw(Model.DrawCallbackFunctionName);
            }

            // Server Request
            options[PropertyConst.FnServerData] = new JRaw(
                "function(sSource, aoData, fnCallback) { "
                + (Model.IsDevelopMode ? "    console.log('[DataTable] URL: ', sSource);" : string.Empty)
                + (Model.IsDevelopMode ? "    console.log('[DataTable] Request: ', aoData);" : string.Empty)
                + "    var ajaxOptions = { 'dataType': 'json', 'type': 'POST', 'url': sSource, 'data': aoData, 'success': fnCallback};"
                + (Model.IsDevelopMode ? "ajaxOptions['success'] = function(data){"
                                              + "    console.log('[DataTable] Response', data);"
                                              + "    if(fnCallback && typeof fnCallback === 'function'){"
                                              + "        fnCallback(data)"
                                              + "    }"
                                              + "};"
                                       : string.Empty)
                + (string.IsNullOrWhiteSpace(Model.BeforeSendFunctionName) ? string.Empty : $"{Model.BeforeSendFunctionName}(aoData);")
                + (string.IsNullOrWhiteSpace(Model.AjaxErrorHandler) ? string.Empty : "ajaxOptions['error'] = " + Model.AjaxErrorHandler + "; ")
                + "    $.ajax(ajaxOptions);" +
                "}");

            // Tools
            if (Model.IsUseTableTools)
            {
                options[PropertyConst.TableTools] = new JRaw("{ 'sSwfPath': '" + Url.AbsoluteContent("~/portal/vendor/datatables-tabletools/swf/copy_csv_xls_pdf.swf") + "' }");

                var tools = Model.IsEnableColVis ? "{extend: 'colvis', text: 'Columns'}," : string.Empty;
                tools += "'copy', 'excel', 'csv', 'pdf', 'print'";
                options[PropertyConst.Buttons] = new JRaw($"[{tools}]");
            }

            // Additional Option
            if (Model.AdditionalOptions.Any())
            {
                foreach (var jsOption in Model.AdditionalOptions)
                {
                    options[jsOption.Key] = new JRaw(jsOption.Value);
                }
            }
        }

        var dataTableOptions = @Html.Raw(options.ToString(Formatting.Indented));

        @if (Model.IsDevelopMode)
        {
            @Html.Raw("console.log('[DataTable] Config', dataTableOptions);")
        }

        var $dataTable = $table.dataTable(dataTableOptions);

        // Col filters
        @if (Model.IsUseColumnFilter)
        {
            @Html.Raw("$dataTable.columnFilter(" + Model.ColumnFilter + ");")
        }

        // Global Variable
        @if (!string.IsNullOrWhiteSpace(Model.GlobalJsVariableName))
        {
            @Html.Raw("window['" + Model.GlobalJsVariableName + "'] = $dataTable;")
        }
    })();
</script>
```

### Customising column rendering
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

[HttpPost]
ActionResult GetMessagesDataRows(DataTablesParam param)
{
    IQueryable<Message> messages = db.Query<Message>();
    IQueryable<MessageViewModel> messageViewModels = messages.Select(m => new MessageViewModel {
        CreatedDate = m.CreatedDate,
        User = m.User.Name,
        Text = m.Text
    });

    return DataTablesResult.Create(messages, param, x => new  {
        CreatedDate = x.CreatedDate.ToFriendlyTimeString(), 
        User = string.Format("<a href='/users/{0}'>{1}</a>", r.UserEnt.Id, r.UserEnt.Name)
    });
});
```

### Specifying Initial Search Values
```csharp
model
	.FilterOn("Position", new { sSelector = "#custom-filter-placeholder-position" }, new { sSearch = "Tester" }).Select("Engineer", "Tester", "Manager")

	.FilterOn("Id", null, new { sSearch = "2~4", bEscapeRegex = false }).NumberRange()

	.FilterOn("IsAdmin", null, new { sSearch = "False" }).Select("True","False")

	.FilterOn("Salary", new { sSelector = "#custom-filter-placeholder-salary" }, new { sSearch = "1000~100000" }).NumberRange();

model.StateSave = false;

// ... other configs
```