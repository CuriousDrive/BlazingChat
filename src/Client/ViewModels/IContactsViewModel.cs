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
        public int ContactsCount { get; set; }
        
        //Next two methods are gonna load 20,000 contacts
        public Task LoadAllContactsDemo();
        public Task LoadOnlyVisibleContactsDemo(int startIndex, int numberOfUsers);
        
        //Next two methods are gonna load the actual contacts
        public Task LoadContactsCount();
        public Task LoadAllContactsDB();
        public Task LoadOnlyVisibleContactsDB(int startIndex, int numberOfUsers);

    }
}
