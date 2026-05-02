using Domain.Repository;
using Domain.Repository.User;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repository;
using Repository.Repository.User;

namespace IoC;

public static class NativeInjector
{
    public static void ConfigureInjections(this IServiceCollection services)
    {
        #region .: USER :.
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserSecurityInfoRepository, UserSecurityInfoRepository>();
        services.AddScoped<IUserHistoricRepository, UserHistoricRepository>();
        services.AddScoped<IUserHistoricService, UserHistoricService>();
        #endregion

        #region .: AUTH / TOKENS :.
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        #endregion

        #region .: COMPANY :.
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICompanyDocumentRepository, CompanyDocumentRepository>();
        services.AddScoped<ICompanyMemberRepository, CompanyMemberRepository>();
        services.AddScoped<ICompanyService, CompanyService>();
        #endregion

        #region .: RBAC :.
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IRbacService, RbacService>();
        services.AddScoped<IRoleService, RoleService>();
        #endregion

        #region .: FILE STORAGE / EMAIL :.
        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IEmailService, EmailService>();
        #endregion
    }
}
