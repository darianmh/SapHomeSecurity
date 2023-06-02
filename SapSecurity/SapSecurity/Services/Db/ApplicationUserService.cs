using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using SapSecurity.Infrastructure.Repositories;
using SapSecurity.Model;
using SapSecurity.Services.Caching;
using SapSecurity.Services.Mapper;
using SapSecurity.Services.Tools;
using SapSecurity.ViewModel;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SapSecurity.Services.Db;

public class ApplicationUserService : IApplicationUserService
{

    #region Fields

    private readonly IApplicationUserRepository _applicationUserRepository;
    private readonly string _secret = "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr";
    private readonly IMapper _mapper;

    #endregion
    #region Methods

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _applicationUserRepository.GetByIdAsync(id);
    }

    public async Task<LoginResponseViewModel?> LoginAsync(LoginViewModel? loginModel)
    {
        if (loginModel == null) return null;
        var passwordHash = PasswordHasher.Hash(loginModel.Password);
        var user = await GetUserByUserAndPass(loginModel.UserName, passwordHash);
        if (user == null) return null;
        var token = GenerateToken(user.ApplicationUser, user);
        return new LoginResponseViewModel(token, user.ApplicationUser.Id);
    }


    public async Task<LoginInfo?> GetUserByUserAndPass(string userName, string passwordHash)
        => await _applicationUserRepository.GetUserByUserAndPass(userName, passwordHash);

    public int GetLoggedInId(string token)
    {
        var principal = ValidateToken(token);
        return Convert.ToInt32(principal.FindFirst("LoggedInId")?.Value);
    }

    public async Task<List<UserViewModel>> GetAllUsers()
    {
        var allUsers=await _applicationUserRepository.GetAllAsync();
        return allUsers.Select(x=>_mapper.Map(x)).ToList();
    }

    public string? GetUserId(string? token)
    {
        if (token == null) return null;
        var principal = ValidateToken(token);
        return principal.FindFirst("UserId")?.Value;
    }
    public ClaimsPrincipal ValidateToken(string jwtToken)
    {
        IdentityModelEventSource.ShowPII = true;

        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret))
        };

        ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out _);

        return principal;
    }
    public async Task<bool> SetSecurityStatus(string user, bool securityState)
    {
        var userModel = await GetByIdAsync(user);
        if (userModel == null) return false;
        userModel.SecurityIsActive = securityState;
        _applicationUserRepository.Update(userModel);
        await _applicationUserRepository.SaveChangeAsync();
        CacheManager.SetUserSecurityActivate(user, securityState);
        return true;
    }

    public async Task<bool> GetSecurityStatus(string user)
    {
        var userModel = await GetByIdAsync(user);
        return userModel?.SecurityIsActive ?? false;
    }

    #endregion
    #region Utilities

    private string GenerateToken(ApplicationUser user, LoginInfo login)
    {

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login.UserName),
            new Claim("UserId", user.Id),
            new Claim("LoggedInId", login.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };


        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddDays(2),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    #endregion
    #region Ctor

    public ApplicationUserService(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _mapper = mapper;
        var scope = serviceScopeFactory.CreateScope();
        _applicationUserRepository = scope.ServiceProvider.GetService<IApplicationUserRepository>();
    }


    #endregion


}