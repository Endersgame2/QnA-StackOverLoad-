namespace QnA.Models
{
    public class QuestionSummaryViewModel
    {
        public int QuestionId { get; set; }
        public List<int> TagIds { get; set; }
        public string Title { get; set; }
        public string User { get; set; }
        public int AnswerCount { get; set; }
        public DateTime TimeStampUtc { get; set; }
    }
}
