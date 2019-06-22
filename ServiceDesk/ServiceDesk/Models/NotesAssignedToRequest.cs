using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models
{
    /// <summary>Provides foreign keys of requests and notes assigned to them.</summary>
    public class NotesAssignedToRequest
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }
        /// <summary>Gets or sets the request identifier.</summary>
        [Display(Name = "Request")]
        public int RequestId { get; set; }
        /// <summary>Gets or sets the request using its identifier as a foreign key.</summary>
        [ForeignKey("RequestId")]
        public virtual Request Request { get; set; }
        /// <summary>Gets or sets the note identifier.</summary>
        [Display(Name = "RequestNote")]
        public int RequestNoteId { get; set; }
        /// <summary>Gets or sets the note using its identifier as a foreign key.</summary>
        [ForeignKey("RequestNoteId")]
        public virtual RequestNote RequestNote { get; set; }


    }
}
