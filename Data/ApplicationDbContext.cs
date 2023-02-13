using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using VerdonFileManager.Models;
using VerdonFileManager.Services;

namespace VerdonFileManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

            Database.EnsureCreatedAsync();
            if (Database.GetPendingMigrationsAsync().Result.Count() > 0)
                Database.MigrateAsync();

        }
        
        public virtual DbSet<AppUser> User { get; set; }
        public virtual DbSet<IdentityRole> Role { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<FileData> FileData { get; set; }


    }


}