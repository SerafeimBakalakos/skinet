using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) // Be specific with the type to avoid bugs
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Without the next line, Identity will cause issues with the primary key for the ID of the AppUser
            base.OnModelCreating(builder);
        }
    }
}