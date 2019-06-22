using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models
{
    /// <summary>Represents a request's classification.</summary>
    public class Classification
    {
        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }
        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

    }
}
