using QnA.Enums;
using QnA.Models.Shared;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QnA.Models.Question
{
    public class QuestionViewModel
    {
        public NotificationViewModel? Notification { get; set; }
        public CommentDetailsViewModel? CommentDetails { get; set; }
        public QuestionViewModel()
        {
            Notification = new NotificationViewModel();
            Answers = new List<AnswerViewModel>();
            CommentDetails = new CommentDetailsViewModel();
        }
        [Required]
        public int QuestionId { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? User { get; set; }
        public List<string>? Tags { get; set; }
        [Required]
        [DisplayName("Your answer")]
        public string Answer { get; set; }
        public List<AnswerViewModel>? Answers { get; set; }
        public int? UpVoteCount { get; set; }
        public int? DownVoteCount { get; set; }
        public VoteType CurrentUserVoteType { get; set; }
        public bool? IsCurrentUserAuthor { get; set; }
        public AnswerViewModel? AcceptedAnswer { get; set; }
    }
}
