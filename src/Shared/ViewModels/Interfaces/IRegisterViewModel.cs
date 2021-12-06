using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public interface IRegisterViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string ReenterPassword { get; set; }
        public Task RegisterUser();
    }
}