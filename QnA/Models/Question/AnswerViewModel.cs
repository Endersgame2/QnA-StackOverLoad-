using QnA.Enums;
using QnA.Models.Shared;

namespace QnA.Models.Question
{
    public class AnswerViewModel
    {
        public CommentDetailsViewModel CommentDetails { get; set; }
        public AnswerViewModel()
        {
            CommentDetails = new CommentDetailsViewModel();
        }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string User { get; set; }
        public string Answer { get; set; }
        public int? UpVoteCount { get; set; }
        public int? DownVoteCount { get; set; }
        public VoteType CurrentUserVoteType { get; set; }
    }
}
