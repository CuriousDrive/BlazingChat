using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public class Contact
    {
        public int ContactId { get; private init; }
        public string FirstName { get; private init; }
        public string LastName { get; private init; }
        public string EmailAddress { get; private init; }

        public static implicit operator Contact(User user) =>
            new()
            {
                ContactId = (int)user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress
            };

        public static implicit operator User(Contact contact) =>
            new()
            {
                UserId = contact.ContactId,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                EmailAddress = contact.EmailAddress
            };
    }
}
