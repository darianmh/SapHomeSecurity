using SapSecurity.Model;

namespace SapSecurity.Infrastructure.Repositories;

public interface IApplicationUserRepository : IBaseRepository<ApplicationUser, string>
{
    Task<LoginInfo?> GetUserByUserAndPass(string userName, string passwordHash);
}