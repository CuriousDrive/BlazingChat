using System;
using System.Collections.Generic;
using System.Text;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public interface ILoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        

    }
}