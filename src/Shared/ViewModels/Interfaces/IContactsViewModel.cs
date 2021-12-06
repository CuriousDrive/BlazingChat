using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public interface IContactsViewModel
    {
        public List<Contact> Contacts { get; set; }
        public int ContactsCount { get; set; }

        //Next two methods are gonna load 20,000 contacts
        public Task LoadAllContactsDemo();
        public Task LoadOnlyVisibleContactsDemo(int startIndex, int numberOfUsers);
        public void LoadContactsCountDemo();

        //Next two methods are gonna load the actual contacts
        public Task LoadContactsCountDB();
        public Task LoadAllContactsDB();
        public Task LoadOnlyVisibleContactsDB(int startIndex, int numberOfUsers);

    }
}
