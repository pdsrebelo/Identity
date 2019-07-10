using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OneThird.Domain.Entities;
using OneThird.Repository.Repositories;
using OneThird.Repository.Repositories.Interfaces;
using RepoNs = OneThird.Repository.Entities;

namespace OneThird.Service.Identity
{
    public class UserEmailStore : UserStore, IUserEmailStore<User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserEmailStore(UserRepository userRepository, IMapper mapper) : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoUsers = await _userRepository.Search(new RepoNs.UserSearchCriteria { NormalizedEmail = normalizedEmail });
            var users = _mapper.Map<IEnumerable<RepoNs.User>, IEnumerable<User>>(repoUsers);

            return users.FirstOrDefault();
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }
    }
}