using ServiceDesk.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models
{
    /// <summary>Represents a request.</summary>
    public class Request
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }


        /// <summary>Gets or sets the title.</summary>
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        /// <summary>Gets or sets the content.</summary>
        [Required]
        public string Content { get; set; }
        /// <summary>Gets or sets the status.</summary>
        public RequestStatus Status { get; set; }
        /// <summary>Gets or sets the submit date.</summary>
        [Required]
        [Display(Name ="Submit Date")]
        [DataType(DataType.Date)]
        public DateTime SubmitDate { get; set; }
        /// <summary>Gets or sets the date of closing request.</summary>
        [Display(Name = "Close Date")]
        [DataType(DataType.Date)]
        public DateTime? ClosedDate { get; set; }
        /// <summary>Gets or sets the team identifier.</summary>
        [Required]
        [Display(Name = "Team")]
        public int TeamId { get; set; }
        /// <summary>Gets or sets the team using its identifier as a foreign key.</summary>
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }
        /// <summary>Gets or sets the classification identifier.</summary>
        //[Required]
        [Display(Name = "Classification")]
        public int ClassificationId { get; set; }
        /// <summary>Gets or sets the classification using its identifier as a foreign key.</summary>
        [ForeignKey("ClassificationId")]
        public virtual Classification Classification { get; set; }
        /// <summary>Gets or sets the requestor user identifier.</summary>
        [Display(Name = "Requestor")]
        public string RequestorId { get; set; }
        /// <summary>Gets or sets the requestor user using its identifier as a foreign key.</summary>
        [ForeignKey("RequestorId")]
        public virtual ApplicationUser Requestor { get; set; }
        /// <summary>Gets or sets the assignee user identifier.</summary>
        [Display(Name = "Assignee")]
        public string AssigneeId { get; set; }
        /// <summary>Gets or sets the assignee user using its identifier as a foreign key.</summary>
        [ForeignKey("AssigneeId")]
        public virtual ApplicationUser Assignee { get; set; }
        /// <summary>Gets or sets the image identifier.</summary>
        [Display(Name = "Image")]
        public int? ImageId { get; set; }
        /// <summary>Gets or sets the image using its identifier as a foreign key.</summary>
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }

    }
}
