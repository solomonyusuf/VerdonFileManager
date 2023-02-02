using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VerdonFileManager.Data;
using VerdonFileManager.Models;

namespace VerdonFileManager.Services
{
    public class UserService
    {
        public readonly IHttpContextAccessor _cont;
        public readonly UserManager<AppUser> _user;
        public readonly ApplicationDbContext _db;
        public UserService(IHttpContextAccessor cont, UserManager<AppUser> user, ApplicationDbContext db)
        {
            _cont = cont;
            _user = user;
            _db = db;
        }
        public async Task<AppUser> CurrentUser()
        {
            var ur = new AppUser();
            var res = _cont.HttpContext.User.Identity.Name;
            if(res != null)
            {
                ur = await _user.FindByEmailAsync(res);
                
            }
            return ur;
        }
    }
}
//var usr = await _db.User.Where(x => res.Equals(x.Email)).ToListAsync();