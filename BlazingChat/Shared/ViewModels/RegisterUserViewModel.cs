using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace BlazingChat.Shared.ViewModels
{
    public class RegisterUserViewModel
    {
        
        [Required(ErrorMessage = "email address can't be empty")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage="please enter valid email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage ="password can't be empty")]
        public string Password { get; set; }

        [Required(ErrorMessage ="confirm password can't be empty")]
        public string ConfirmPassword { get; set; }
         
    }
}