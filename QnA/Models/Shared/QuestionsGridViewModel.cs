namespace QnA.Models.Shared
{
    public class QuestionsGridViewModel
    {
        public bool IsAdminView { get; set; } = false;
        public List<QuestionSummaryViewModel> Questions { get; set; }
        public QuestionsGridViewModel()
        {
            IsAdminView = false;
            Questions = new List<QuestionSummaryViewModel>();
        }
    }
}
