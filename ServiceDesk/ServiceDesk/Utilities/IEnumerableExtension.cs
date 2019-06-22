using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServiceDesk.Utilities
{
    /// <summary>Provides methods for creation of SelectListItem collection.</summary>
    public static class IEnumerableExtension
    {

        /// <summary>Converts to selectlistitemstring.</summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="items">Collection of string objects.</param>
        /// <param name="selectedValue">Selected element from collection.</param>
        /// <returns>Collection of SelectListItem objects.</returns>
        public static IEnumerable<SelectListItem> ToSelectListItemString<T>(this IEnumerable<T> items, string selectedValue)
        {
            if (selectedValue == null)
            {
                selectedValue = "";
            }
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }

        /// <summary>Converts to selectlistitemstring.</summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="items">Collection of int objects.</param>
        /// <param name="selectedValue">Selected element from collection.</param>
        /// <returns>Collection of SelectListItem objects.</returns>
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {

            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }
    }
}

