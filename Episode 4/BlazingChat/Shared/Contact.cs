namespace BlazingChat.Shared
{
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Contact()
        {

        }

        public Contact(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
        }
    }
}