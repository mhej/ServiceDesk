using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.Models;

namespace ServiceDesk.Data
{
    /// <summary>Maps model classes to the database.</summary>
    /// <seealso cref="Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext" />
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<ClassificationAssignedToTeam> ClassificationAssignedToTeam { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<AdminAssignedToTeam> AdminAssignedToTeam { get; set; }
        public DbSet<RequestNote> RequestNotes { get; set; }
        public DbSet<NotesAssignedToRequest> NotesAssignedToRequest { get; set; }
        public DbSet<ImagesAssignedToRequest> ImagesAssignedToRequest { get; set; }
        public DbSet<Image> Image { get; set; }

    }
}
