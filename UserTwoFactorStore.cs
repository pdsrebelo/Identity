using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OneThird.Domain.Entities;
using OneThird.Repository.Repositories;

namespace OneThird.Service.Identity
{
    public class UserTwoFactorStore : UserStore, IUserTwoFactorStore<User>
    {
        public UserTwoFactorStore(UserRepository userRepository, IMapper mapper) : base(userRepository, mapper)
        {
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }
    }
}