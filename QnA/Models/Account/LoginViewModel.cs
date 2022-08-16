using QnA.Models.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace QnA.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("Username or Email")]
        public string Key { get; set; }
        [Required]
        public string Password { get; set; }
        public NotificationViewModel? Notification { get; set; }
        public LoginViewModel()
        {
            Notification = new NotificationViewModel();
        }
    }
}
