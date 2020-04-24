using System;
using System.Collections.Generic;
using System.Text;
using BlazingChat.Shared.Models;

namespace BlazingChat.Shared.ViewModels
{
    public class ChatViewModel
    {    
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string TimeSinceLastMessage { get; set; }
        public string LastMessage { get; set; }

        public ChatViewModel()
        {

        }

        public ChatViewModel(string firstName, string lastName, string emailAddress)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.EmailAddress = emailAddress;
            LastMessage = "Donec id elit non mi porta gravida at eget metus.";
            TimeSinceLastMessage = "1 day ago";
        }

        public static implicit operator ChatViewModel(User user)
        {
            return new ChatViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress
            };
        }

        public static implicit operator User(ChatViewModel chatViewModel)
        {
            return new User
            {
                FirstName = chatViewModel.FirstName,
                LastName = chatViewModel.LastName,
                EmailAddress = chatViewModel.EmailAddress
            };
        }
    }


}
