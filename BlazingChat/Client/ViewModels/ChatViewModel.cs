using System;
using System.Collections.Generic;
using System.Text;

namespace BlazingChat.Client.ViewModels
{
    public class ChatViewModel
    {    
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string TimeSinceLastMessage { get; set; }
        public string LastMessage { get; set; }
    }
}
