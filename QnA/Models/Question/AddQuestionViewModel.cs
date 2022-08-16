using Microsoft.AspNetCore.Mvc.Rendering;
using QnA.Models.Shared;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QnA.Models.Question
{
    public class AddQuestionViewModel
    {
        [Required]
        public string Title { get; set; }
        public string? Text { get; set; }
        [DisplayName("Tags")]
        public List<int>? TagIds { get; set; }
        public MultiSelectList? TagOptions { get; set; }
        public NotificationViewModel? Notification { get; set; }
        public AddQuestionViewModel()
        {
            Notification = new NotificationViewModel();
        }
    }
}
