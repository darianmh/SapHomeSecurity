using SapSecurity.Model;
using SapSecurity.ViewModel;

namespace SapSecurity.Services.Db;

public interface IApplicationUserService
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<LoginResponseViewModel?> LoginAsync(LoginViewModel? loginModel);
    string? GetUserId(string? token);
    /// <summary>
    /// active or de active security
    /// </summary>
    /// <param name="user"></param>
    /// <param name="securityState">1 active 0 de active</param>
    /// <returns></returns>
    Task<bool> SetSecurityStatus(string user, bool securityState);
    /// <summary>
    /// get security status
    /// 1 active 0 de active
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<bool> GetSecurityStatus(string user);

    Task<LoginInfo?> GetUserByUserAndPass(string userName, string passwordHash);
    int GetLoggedInId(string token);
}