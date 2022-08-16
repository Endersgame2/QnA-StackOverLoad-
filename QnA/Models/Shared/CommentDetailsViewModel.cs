using QnA.Models.Question;

namespace QnA.Models.Shared
{
    public class CommentDetailsViewModel
    {
        public CommentDetailsViewModel()
        {
            Comments = new List<CommentViewModel>();
            AddComment = new CommentViewModel();
        }
        public List<CommentViewModel>? Comments { get; set; }
        public CommentViewModel? AddComment { get; set; }
    }
}
