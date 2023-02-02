using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VerdonFileManager.Models;
using VerdonFileManager.Services;

namespace VerdonFileManager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }
        
        public virtual DbSet<AppUser> User { get; set; }
        public virtual DbSet<IdentityRole> Role { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<FileData> FileData { get; set; }

    }


}