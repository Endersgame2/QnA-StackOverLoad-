using System.ComponentModel.DataAnnotations;

namespace QnA.Models.Question
{
    public class CommentViewModel
    {
        [Required]
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public string? Comment { get; set; }
        public string? User { get; set; }
    }
}
