#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DataTableFilterType.cs </Name>
//         <Created> 26/09/17 11:15:43 PM </Created>
//         <Key> b9e6d119-d3b7-4d62-aed0-731c58d17b10 </Key>
//     </File>
//     <Summary>
//         DataTableFilterType.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.ComponentModel.DataAnnotations;

namespace Puppy.DataTable.Constants
{
    public enum FilterType
    {
        [Display(Name = FilterConst.None)]
        None,

        [Display(Name = FilterConst.Select)]
        Select,

        [Display(Name = FilterConst.Text)]
        Text,

        [Display(Name = FilterConst.Checkbox)]
        Checkbox,

        [Display(Name = FilterConst.NumberRange)]
        NumberRange

        // TODO, Currently not support for DateRange and DateTimeRange, because I not support date-range in columnsFilter plugin yet.
        //[Display(Name = FilterConst.DateRange)]
        //DateRange,

        //[Display(Name = FilterConst.DateTimeRange)]
        //DateTimeRange
    }
}