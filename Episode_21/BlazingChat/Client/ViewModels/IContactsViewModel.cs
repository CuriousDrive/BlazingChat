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
        public Task GetContacts();
        public Task<List<Contact>> GetAllContacts();
        public Task<List<Contact>> GetSomeContacts(int startIndex, int numberOfUsers);
    }
}
