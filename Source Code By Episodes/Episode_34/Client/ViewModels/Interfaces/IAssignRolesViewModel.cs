using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public interface IAssignRolesViewModel
    {
        public List<User> UsersWithoutRole { get; set; }

        public Task LoadUsersWithoutRole();
        public Task AssignRole(long userId, string role);
        public Task DeleteUser(long userId);
    }
}
