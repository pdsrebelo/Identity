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
    public class RoleStore : IRoleStore<Role>
    {
        private readonly RoleRepository _roleRepository;
        private readonly IMapper _mapper;

        public RoleStore(RoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoRole = _mapper.Map<Role, RepoNs.Role>(role);
            var result = await _roleRepository.Add(repoRole);

            return result > 0 ? IdentityResult.Success : IdentityResult.Failed();
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoRole = _mapper.Map<Role, RepoNs.Role>(role);
            await _roleRepository.Update(repoRole);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _roleRepository.Delete(role.Id);

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;

            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;

            return Task.FromResult(0);
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoRoles = await _roleRepository.Search(new RepoNs.RoleSearchCriteria { Id = int.Parse(roleId) });
            var roles = _mapper.Map<IEnumerable<RepoNs.Role>, IEnumerable<Role>>(repoRoles);

            return roles.FirstOrDefault();
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var repoRoles = await _roleRepository.Search(new RepoNs.RoleSearchCriteria { NormalizedName = normalizedRoleName });
            var roles = _mapper.Map<IEnumerable<RepoNs.Role>, IEnumerable<Role>>(repoRoles);

            return roles.FirstOrDefault();
        }

        public void Dispose() { }
    }
}
