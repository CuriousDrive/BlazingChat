using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using BlazingChat.Shared.Models;

namespace BlazingChat.Shared.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage ="first name can't be empty")]
        public string FirstName { get; set; }
        [Required(ErrorMessage ="last name can't be empty")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "email address can't be empty")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage="please enter valid email address")]
        public string EmailAddress { get; set; }        
        public DateTime DateOfBirth { get; set; }
        [StringLength(200, MinimumLength = 10, ErrorMessage ="about me should be more than 10 chars and less than 200 chars")]
        public string AboutMe { get; set; }
        public string ProfilePictureUrl { get; set; }
        public long UserId { get; set; }
        public string Password { get; set; }
        public string Source { get; set; }
        public long Notifications { get; set; }
        public long DarkTheme { get; set; }

        public static implicit operator ProfileViewModel(User user)
        {
            return new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                //DateOfBirth = DateTime.FromBinary(BitConverter.ToInt64(user.DateOfBirth, 0)),
                AboutMe = user.AboutMe,

                UserId = user.UserId,
                Password = user.Password,
                Source = user.Source,
                Notifications = (long)user.Notifications,
                DarkTheme = (long)user.DarkTheme
            };
        }

        public static implicit operator User(ProfileViewModel profileViewModel)
        {
            return new User
            {
                FirstName = profileViewModel.FirstName,
                LastName = profileViewModel.LastName,
                EmailAddress = profileViewModel.EmailAddress,
                //DateOfBirth = BitConverter.GetBytes(profileViewModel.DateOfBirth.Ticks),
                AboutMe = profileViewModel.AboutMe,

                UserId = profileViewModel.UserId,
                Password = profileViewModel.Password,
                Source = profileViewModel.Source,
                Notifications = profileViewModel.Notifications,
                DarkTheme = profileViewModel.DarkTheme
            };
        }
    }
}