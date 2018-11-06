using System;
using System.Collections.Generic;
using System.Text;
using GranitHouse.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GranitHouse.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductTypes> ProductTypes { get; set; }

        public DbSet<SpecialTagTypes> SpecialTagTypes { get; set; }
        public DbSet<Products> Products { get; set; }

        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<ProductSelectedForAppointment> ProductSelectedForAppointments { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
