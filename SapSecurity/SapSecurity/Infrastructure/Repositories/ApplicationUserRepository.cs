using Microsoft.EntityFrameworkCore;
using SapSecurity.Model;
using SapSecurity.Data;

namespace SapSecurity.Infrastructure.Repositories;

public class ApplicationUserRepository : BaseRepository<ApplicationUser, string>, IApplicationUserRepository
{
    #region Fields



    #endregion
    #region Methods

    public override Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return DbSet.Include(x => x.LoginInfos).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<LoginInfo?> GetUserByUserAndPass(string userName, string passwordHash)
    {
        return await _context.LoginInfos
            .Include(x => x.ApplicationUser)
            .FirstOrDefaultAsync(x =>
            x.UserName.ToLower() == userName.ToLower() && x.PasswordHash == passwordHash);
    }

    #endregion
    #region Utilities



    #endregion
    #region Ctor


    public ApplicationUserRepository(ApplicationContext context) : base(context)
    {
    }

    #endregion

}