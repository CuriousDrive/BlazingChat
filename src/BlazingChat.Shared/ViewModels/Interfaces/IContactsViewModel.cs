using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public interface IContactsViewModel
    {
        public IEnumerable<Contact> Contacts { get; }
        public int ContactsCount { get; }

        //Next two methods are gonna load the actual contacts
        public Task LoadContactsCountDB();
        public Task LoadAllContactsDB();
        public Task LoadOnlyVisibleContactsDB(int startIndex, int numberOfUsers);

    }
}
