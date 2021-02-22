using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public interface IContactsViewModel
    {
        public List<Contact> Contacts { get; set; }
        public Task<List<Contact>> GetContacts();
        public Task<List<Contact>> GetAllContacts();
        public Task<List<Contact>> GetOnlyVisibleContacts(int startIndex, int numberOfUsers);
        public Task<int> GetContactsCount();
        public Task<List<Contact>> GetVisibleContacts(int startIndex, int numberOfUsers);

    }
}
