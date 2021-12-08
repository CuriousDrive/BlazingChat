using BlazingChat.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public interface IAssignRolesViewModel
    {
        public IEnumerable<User> UsersWithoutRole { get; }

        public Task LoadUsersWithoutRole();
        public Task AssignRole(long userId, string role);
        public Task DeleteUser(long userId);
    }
}
