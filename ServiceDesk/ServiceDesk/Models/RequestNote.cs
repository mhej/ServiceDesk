using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models
{
    /// <summary>Represents a request note.</summary>
    public class RequestNote

    {        
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }
        /// <summary>Gets or sets the status.</summary>
        public string Content { get; set; }
        /// <summary>Gets or sets the submission date.</summary>
        public DateTime Date { get; set; }
        /// <summary>Gets or sets the submission user.</summary>
        [DisplayName("Note User")]
        public ApplicationUser NoteUser { get; set; }

    }
}
