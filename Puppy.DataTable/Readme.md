![Logo](favicon.ico)
# Puppy.DataTable Document
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- This is a flork and continue develop of project [mvc.jquery.datatables]("https://github.com/mcintyre321/mvc.jquery.datatables")
- Improve things:
    + Coding smell
    + Coding Convention
    + Improve query performance
    + Support for real cases

### Changing case sensitivity

```csharp
@using QueryInterceptor

...

public DataTablesResult<TDataTableRow> GetDataTableData(DataTablesParam dataTableParam)
{
    var originalQueryable = ... some code to get your IQueryable<TDataTableRow> ...;
    var caseInsenstitiveQueryable = originalQueryable.InterceptWith(new SetComparerExpressionVisitor(StringComparison.CurrentCultureIgnoreCase));

    return DataTablesResult.Create(caseInsenstitiveQueryable, dataTableParam);
}
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
     vm
        .FilterOn("Position", new { sSelector = "#custom-filter-placeholder-position" }, new { sSearch = "Tester" }).Select("Engineer", "Tester", "Manager")

        .FilterOn("Id", null, new { sSearch = "2~4", bEscapeRegex = false }).NumberRange()

        .FilterOn("IsAdmin", null, new { sSearch = "False" }).Select("True","False")

        .FilterOn("Salary", new { sSelector = "#custom-filter-placeholder-salary" }, new { sSearch = "1000~100000" }).NumberRange();

    vm.StateSave = false;
```