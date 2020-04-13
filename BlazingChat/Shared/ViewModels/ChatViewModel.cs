using System;
using System.Collections.Generic;
using System.Text;

namespace BlazingChat.Shared.ViewModels
{
    public class ChatViewModel
    {    
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string TimeSinceLastMessage { get; set; }
        public string LastMessage { get; set; }

        public ChatViewModel()
        {

        }

        public ChatViewModel(string firstName, string lastName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            LastMessage = "Donec id elit non mi porta gravida at eget metus.";
            TimeSinceLastMessage = "1 day ago";
        }
    }


}
