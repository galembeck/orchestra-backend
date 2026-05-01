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
        #region .: INTERNAL INJECTIONS :.

        #region .: USER :.
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        services.AddScoped<IUserAddressService, UserAddressService>();

        #endregion .: USER :.

        #region .: USER SECURITY INFO :.

        services.AddScoped<IUserSecurityInfoRepository, UserSecurityInfoRepository>();

        #endregion .: USER SECURITY INFO :.

        #region .: USER HISTORIC :.

        services.AddScoped<IUserHistoricRepository, UserHistoricRepository>();
        services.AddScoped<IUserHistoricService, UserHistoricService>();

        #endregion .: USER HISTORIC :.

        #region .: AUTH :.

        services.AddScoped<IAuthService, AuthService>();

        #endregion .: AUTH :.

        #region .: TOKENS :.

        services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        #endregion .: TOKENS :.

        #region .: FILE STORAGE :.

        services.AddScoped<IFileStorageService, FileStorageService>();

        #endregion .: FILE STORAGE :.

        #region .: EMAIL :.

        services.AddScoped<IEmailService, EmailService>();

        #endregion .: EMAIL :.

        #region .: JOBS :.

        services.AddScoped<ITrackingCodeEmailJob, TrackingCodeEmailJob>();

        #endregion .: JOBS :.

        #endregion .: INTERNAL INJECTIONS :.
    }
}
