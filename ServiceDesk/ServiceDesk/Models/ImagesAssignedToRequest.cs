using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceDesk.Models
{
    /// <summary>Provides foreign keys of requests and images assigned to them.</summary>
    public class ImagesAssignedToRequest
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }
        /// <summary>Gets or sets the request identifier.</summary>
        [Display(Name = "Request")]
        public int? RequestId { get; set; }
        /// <summary>Gets or sets the request using its identifier as a foreign key.</summary>
        [ForeignKey("RequestId")]
        public virtual Request Request { get; set; }
        /// <summary>Gets or sets the image identifier.</summary>
        [Display(Name = "Image")]
        public int ImageId { get; set; }
        /// <summary>Gets or sets the image using its identifier as a foreign key.</summary>
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }
    }
}
