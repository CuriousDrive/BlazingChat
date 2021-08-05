using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }


        public Contact()
        {

        }
        public Contact(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public Contact(int contactId, string firstName, string lastName)
        {
            this.ContactId = contactId;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        //operators
        public static implicit operator Contact(User user)
        {
            return new Contact
            {
                ContactId = (int)user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress
            };
        }
        public static implicit operator User(Contact contact)
        {
            return new User
            {
                UserId = contact.ContactId,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                EmailAddress = contact.EmailAddress
            };
        }
    }
}