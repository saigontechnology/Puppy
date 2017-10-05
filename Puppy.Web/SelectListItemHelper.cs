#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SelectListItemHelper.cs </Name>
//         <Created> 06/10/17 2:18:15 AM </Created>
//         <Key> f47a987a-2420-4a36-bd60-8a1ba6613b64 </Key>
//     </File>
//     <Summary>
//         SelectListItemHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Mvc.Rendering;
using Puppy.Core.EnumUtils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.Web
{
    public static class SelectListItemHelper
    {
        /// <summary>
        ///     Get Select List Item by Enum 
        /// </summary>
        /// <param name="enumType">     </param>
        /// <param name="selectedValue"> Enum name of selected item </param>
        /// <returns></returns>
        public static List<SelectListItem> GetEnumSelectList(this Type enumType, string selectedValue = null)
        {
            var listValueLabel = enumType.GetEnumValueLabelPair();

            List<SelectListItem> listSelectListItem = listValueLabel.Select(x => new SelectListItem
            {
                Value = x.Value,
                Text = x.Label,
                Selected = x.Value == selectedValue
            }).ToList();

            return listSelectListItem;
        }
    }
}