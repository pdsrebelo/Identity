using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.AspNetCore.Identity;
using OneThird.Domain.Entities;
using OneThird.Repository.Infrastructure;
using OneThird.Repository.Repositories;
using OneThird.Repository.Repositories.Interfaces;
using RepoNs = OneThird.Repository.Entities;

namespace OneThird.Service.Identity
{
    public class UserRoleStore : UserStore, IUserRoleStore<User>
    {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleStore(ConnectionFactory connectionFactory, UserRepository userRepository,
                             IUserRoleRepository userRoleRepository, IMapper mapper)
            : base(userRepository, mapper)
        {
            _connectionFactory = connectionFactory;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _connectionFactory.GetConnection)
            {
                // TODO: CHANGE THIS!
                var queryResults = await connection.QueryAsync<Role>(
                    "SELECT r.* FROM [Roles] r INNER JOIN [UserRoles] ur ON ur.[RoleId] = r.Id " +
                        "WHERE ur.IsDeleted = 0 AND ur.UserId = @userId",
                    new { userId = user.Id });

                var selectedResults = queryResults.Select(r => r.Name).ToList();

                return selectedResults;
            }
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = _connectionFactory.GetConnection)
            {
                // TODO: CHANGE THIS!
                var queryResults = await connection.QueryAsync<User>("SELECT u.* FROM [Users] u " +
                                                                     "INNER JOIN [UserRoles] ur ON ur.[UserId] = u.[Id] " +
                                                                     "INNER JOIN [Roles] r ON r.[Id] = ur.[RoleId] " +
                                                                     "WHERE r.[NormalizedName] = @normalizedName",
                    new { normalizedName = roleName.ToUpper() });

                return queryResults.ToList();
            }
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roleRepository = new RoleRepository(_connectionFactory);
            var role = (await roleRepository.Search(new RepoNs.RoleSearchCriteria { NormalizedName = roleName.ToUpper() })).FirstOrDefault();

            if (role == null)
                return false;

            return (await _userRoleRepository.Search(new RepoNs.UserRoleSearchCriteria { UserId = user.Id, RoleId = role.Id })).Any();
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            var roleRepository = new RoleRepository(_connectionFactory);
            var roleNormalizedName = roleName.ToUpper();
            var role = (await roleRepository.Search(new RepoNs.RoleSearchCriteria { NormalizedName = roleNormalizedName })).FirstOrDefault();
            int roleId;

            if (role == null)
            {
                roleId = await roleRepository.Add(new RepoNs.Role
                {
                    Name = roleName,
                    NormalizedName = roleNormalizedName
                });
            }
            else
            {
                roleId = role.Id;
            }

            var existingUser = (await _userRoleRepository.Search(new RepoNs.UserRoleSearchCriteria { UserId = user.Id })).FirstOrDefault();
            if (existingUser != null)
            {
                await _userRoleRepository.Add(new RepoNs.UserRole
                {
                    UserId = existingUser.Id,
                    RoleId = roleId,
                    StampDate = user.StampDate,
                    StampUser = user.StampUser
                });
            }
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var roleRepository = new RoleRepository(_connectionFactory);
            var role = (await roleRepository.Search(new RepoNs.RoleSearchCriteria { NormalizedName = roleName.ToUpper() })).FirstOrDefault();

            if (role == null)
                return;

            var userRoles = await _userRoleRepository.Search(new RepoNs.UserRoleSearchCriteria { UserId = user.Id, RoleId = role.Id });
            foreach (var userRole in userRoles)
            {
                userRole.IsDeleted = true;
                await _userRoleRepository.Update(userRole);
            }
        }
    }
}