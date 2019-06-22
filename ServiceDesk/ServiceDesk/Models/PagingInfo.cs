using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models
{

    /// <summary>Provides data required for pagination in the Views.</summary>
    public class PagingInfo
    {
        /// <summary>Gets or sets amount of all items.</summary>
        public int TotalItems { get; set; }
        /// <summary>Gets or sets amount of  items per one page.</summary>
        public int ItemsPerPage { get; set; }
        /// <summary>Gets or sets number of the current page.</summary>
        public int CurrentPage { get; set; }
        /// <summary>Gets amount of all required pages.</summary>
        public int TotalPages { get { return (int)Math.Ceiling(1.0 * TotalItems / ItemsPerPage); } }
        /// <summary>Gets or sets the URL including page number and search criteria.</summary>
        public string urlParam { get; set; }
    }
}
