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

using System;
using System.ComponentModel.DataAnnotations;

namespace Puppy.DataTable.Constants
{
    public enum FilterType
    {
        /// <summary>
        ///     Not show any input in Column Filter 
        /// </summary>
        [Display(Name = FilterConst.None)]
        None,

        /// <summary>
        ///     Display as Dropdown list in Column Filter 
        /// </summary>
        [Display(Name = FilterConst.Select)]
        Select,

        /// <summary>
        ///     Display as Free Text input in Column Filter 
        /// </summary>
        [Display(Name = FilterConst.Text)]
        Text,

        [Obsolete("I'm not support for this filter type by default, you need self implement UI for checkbox type in the columnFilter.js")]
        [Display(Name = FilterConst.Checkbox)]
        Checkbox,

        [Obsolete("I'm not support for this filter type by default, you need self implement UI for number-range type in the columnFilter.js")]
        [Display(Name = FilterConst.NumberRange)]
        NumberRange,

        [Obsolete("I'm not support for this filter type by default, you need self implement UI for date-range type in the columnFilter.js")]
        [Display(Name = FilterConst.DateRange)]
        DateRange,

        [Obsolete("I'm not support for this filter type by default, you need self implement UI for datetime-range type in the columnFilter.js")]
        [Display(Name = FilterConst.DateTimeRange)]
        DateTimeRange
    }
}