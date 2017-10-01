#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DataTablePropertyName.cs </Name>
//         <Created> 26/09/17 11:01:59 PM </Created>
//         <Key> 01cd7f90-55a2-4ede-a3c7-076d884e968d </Key>
//     </File>
//     <Summary>
//         DataTablePropertyName.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.DataTable.Constants
{
    public static class ConfigConst
    {
        public const string DefaultConfigSection = "DataTable";
    }

    public static class PropertyConst
    {
        // Config
        public const string RowId = "DT_RowID";

        public const string Selector = "sSelector";
        public const string Sortable = "bSortable";
        public const string Visible = "bVisible";
        public const string Searchable = "bSearchable";
        public const string Render = "mRender";
        public const string ClassName = "className";
        public const string Width = "width";
        public const string Targets = "aTargets";
        public const string Sorting = "aaSorting";
        public const string IsProcessing = "bProcessing";
        public const string IsStateSave = "stateSave";
        public const string StateDuration = "stateDuration";
        public const string IsServerSide = "bServerSide";
        public const string IsFilter = "bFilter";
        public const string Dom = "sDom";
        public const string IsResponsive = "responsive";
        public const string IsAutoWidth = "bAutoWidth";
        public const string AjaxSource = "sAjaxSource";
        public const string ColumnDefine = "aoColumnDefs";
        public const string SearchCols = "aoSearchCols";
        public const string LengthMenuDefine = "aLengthMenu";
        public const string FnDrawCallback = "fnDrawCallback";
        public const string FnServerData = "fnServerData";
        public const string TableTools = "oTableTools";
        public const string Buttons = "buttons";
        public const string DeferRender = "deferRender";

        // Request
        public const string DisplayStart = "iDisplayStart";

        public const string DisplayLength = "iDisplayLength";
        public const string Columns = "iColumns";
        public const string Search = "sSearch";
        public const string EscapeRegex = "bEscapeRegex";
        public const string SortingCols = "iSortingCols";
        public const string Echo = "sEcho";
        public const string ColumnNames = "sColumnNames";
        public const string SearchValues = "sSearchValues";
        public const string SortCol = "iSortCol";
        public const string SortDir = "sSortDir";
        public const string EscapeRegexColumns = "bEscapeRegexColumns";

        // Response
        public const string TotalRecords = "iTotalRecords";

        public const string TotalDisplayRecords = "iTotalDisplayRecords";
        public const string Data = "aaData";

        // Language
        public const string Language = "language";

        public const string LanguageCode = "oLanguage";
        public const string SearchSelector = "search";
        public const string SearchPlaceholder = "sSearchPlaceholder";
        public const string Processing = "sProcessing";
        public const string LengthMenu = "sLengthMenu";
        public const string LengthMenuSelector = "lengthMenu";
        public const string ZeroRecords = "sZeroRecords";
        public const string Info = "sInfo";
        public const string InfoEmpty = "sInfoEmpty";
        public const string InfoFiltered = "sInfoFiltered";
        public const string InfoPostFix = "sInfoPostFix";
        public const string Url = "sUrl";
        public const string Paginate = "oPaginate";

        // Paging
        public const string First = "sFirst";

        public const string Previous = "sPrevious";
        public const string Next = "sNext";
        public const string Last = "sLast";
    }

    public static class FilterConst
    {
        // Types
        public const string None = "none";

        public const string Select = "select";
        public const string Text = "text";
        public const string Checkbox = "checkbox";
        public const string Number = "number";
        public const string NumberRange = "number-range";
        public const string DateRange = "date-range";
        public const string DateTimeRange = "datetime-range";

        // Property
        public const string Type = "type";

        public const string Values = "values";
        public const string PlaceHolder = "sPlaceHolder";
        public const string UseColVis = "bUseColVis";
        public const string Columns = "aoColumns";
    }

    public static class DataConst
    {
        public const string EmptyArray = "[]";
        public const string True = "True";
        public const string TrueLower = "true";
        public const string False = "False";
        public const string FalseLower = "false";
        public const string Null = "null";
    }

    public static class ConditionalCost
    {
        public const string StartsWith = "StartsWith";
        public const string EndsWith = "EndsWith";
        public const string Contain = "Contains";
        public const string Equal = "Equals";
    }
}