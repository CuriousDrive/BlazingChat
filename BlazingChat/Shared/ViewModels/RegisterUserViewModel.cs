using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using BlazingChat.Shared.Models;

namespace BlazingChat.Shared.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "email address can't be empty")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "please enter valid email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "password can't be empty")]
        public string Password { get; set; }

        [Required(ErrorMessage = "confirm password can't be empty")]
        public string ConfirmPassword { get; set; }

        public static implicit operator RegisterViewModel(User user)
        {
            return new RegisterViewModel
            {
                EmailAddress = user.EmailAddress,
                Password = user.Password,
                ConfirmPassword = user.Password
            };
        }

        public static implicit operator User(RegisterViewModel registerViewModel)
        {
            return new User
            {
                EmailAddress = registerViewModel.EmailAddress,
                Password = registerViewModel.Password
            };
        }
    }
}