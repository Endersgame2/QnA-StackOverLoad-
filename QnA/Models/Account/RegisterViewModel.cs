using QnA.Models.Shared;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QnA.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        public NotificationViewModel? Notification { get; set; }
        public RegisterViewModel()
        {
            Notification = new NotificationViewModel();
        }
    }
}
