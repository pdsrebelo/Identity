using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OneThird.Domain.Entities;

namespace OneThird.Service.Identity
{
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        ///     This method registers identity services and stores using the User and Role types.
        /// </summary>
        /// <param name="services"></param>
        public static IdentityBuilder AddIdentityWithStores(this IServiceCollection services)
        {
            return services.AddIdentity<User, Role>()
                .RegisterStores()
                .AddSignInManager<SignInManager<User>>();
        }

        private static IdentityBuilder RegisterStores(this IdentityBuilder builder)
        {
            builder.Services.AddScoped<IUserStore<User>, UserStore>();
            builder.Services.AddScoped<IRoleStore<Role>, RoleStore>();

            builder.Services.AddScoped<IUserRoleStore<User>, UserRoleStore>();
            builder.Services.AddScoped<IUserEmailStore<User>, UserEmailStore>();
            builder.Services.AddScoped<IUserPasswordStore<User>, UserPasswordStore>();
            builder.Services.AddScoped<IUserPhoneNumberStore<User>, UserPhoneNumberStore>();
            builder.Services.AddScoped<IUserTwoFactorStore<User>, UserTwoFactorStore>();

            return builder;
        }
    }
}