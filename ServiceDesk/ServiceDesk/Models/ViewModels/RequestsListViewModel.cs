using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for Request Views and Controller.</summary>
    public class RequestsListViewModel
    {
        /// <summary>Gets or sets the list of requests.</summary>
        public List<Request> Requests { get; set; }
        /// <summary>Gets or sets the title of the searched request.</summary>
        public string SearchTitle { get; set; }
        /// <summary>Gets or sets the classifiction of the searched request.</summary>
        public string SearchClassification { get; set; }
        /// <summary>Gets or sets the team of the searched request.</summary>
        public string SearchTeam { get; set; }
        /// <summary>Gets or sets the requestor of the searched request.</summary>
        public string SearchRequestor { get; set; }
        /// <summary>Gets or sets the assignee of the searched request.</summary>
        public string SearchAssignee { get; set; }
        /// <summary>Gets or sets the status of the searched request.</summary>
        public string SearchStatus { get; set; }
        /// <summary>Gets or sets the start of submit date range of the searched request.</summary>
        public string SearchSubmitDateFrom { get; set; }
        /// <summary>Gets or sets the end of submit date range of the searched request.</summary>
        public string SearchSubmitDateTo { get; set; }
        /// <summary>Gets or sets data required for pagination in the Index View.</summary>
        public PagingInfo PagingInfo { get; set; }
    }
}
