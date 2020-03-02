using System;
using System.Collections.Generic;

namespace BlazingChat.Shared.Models
{
    public partial class Contacts
    {
        public long ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
