using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OneThird.Domain.Entities;
using OneThird.Repository.Repositories;
using RepoNs = OneThird.Repository.Entities;

namespace OneThird.Service.Identity
{
    public class UserStore : IUserStore<User>
    {
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserStore(UserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoUser = _mapper.Map<User, RepoNs.User>(user);
            var result = await _userRepository.Add(repoUser);

            return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoUser = _mapper.Map<User, RepoNs.User>(user);
            await _userRepository.Update(repoUser);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _userRepository.Delete(user.Id);

            return IdentityResult.Success;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoUsers = await _userRepository.Search(new RepoNs.UserSearchCriteria { Id = int.Parse(userId) });
            var users = _mapper.Map<IEnumerable<RepoNs.User>, IEnumerable<User>>(repoUsers);

            return users.FirstOrDefault();
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.Id.ToString());
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoUsers = await _userRepository.Search(new RepoNs.UserSearchCriteria { NormalizedUserName = normalizedUserName });
            var users = _mapper.Map<IEnumerable<RepoNs.User>, IEnumerable<User>>(repoUsers);

            return users.FirstOrDefault();
        }

        #region UserName
        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.UserName);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.UserName = userName;

            return Task.FromResult(0);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedUserName = normalizedName;

            return Task.FromResult(0);
        }
        #endregion

        public void Dispose() { }
    }
}
