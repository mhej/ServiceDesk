using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Utilities
{

    /// <summary>Provides method getting property value.</summary>
    public static class ReflectionExtension
    {
        /// <summary>Gets value of the specified property.</summary>
        /// <typeparam name="T">Generic type.</typeparam>
        /// <param name="item">The selected object.</param>
        /// <param name="propertyName">Name of the specified property of the item.</param>
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
    }
}
