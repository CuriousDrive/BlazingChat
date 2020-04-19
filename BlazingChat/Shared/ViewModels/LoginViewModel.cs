using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

using BlazingChat.Shared.Models;

namespace BlazingChat.Shared.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "email address can't be empty")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "please enter valid email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "password can't be empty")]
        public string Password { get; set; }

        public static implicit operator LoginViewModel(User user)
        {
            return new LoginViewModel
            {
                EmailAddress = user.EmailAddress,
                Password = user.Password
            };
        }

        public static implicit operator User(LoginViewModel loginViewModel)
        {
            return new User
            {
                EmailAddress = loginViewModel.EmailAddress,
                Password = loginViewModel.Password
            };
        }
    }
}