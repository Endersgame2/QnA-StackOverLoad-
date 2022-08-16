using QnA.Enums;
using System.ComponentModel.DataAnnotations;

namespace QnA.Models.Question
{
    public class VoteActionViewModel
    {
        [Required]
        public int QuestionId { get; set; }
        public int? AnswerId { get; set; }
        public VoteType VoteType { get; set; }
    }
}
