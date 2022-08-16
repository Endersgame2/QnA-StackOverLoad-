using QnA.Models.Shared;

namespace QnA.Models
{
    public class AdminViewModel
    {
        public NotificationViewModel? Notification { get; set; }
        public QuestionsGridViewModel? QuestionsGrid { get; set; }
        public AdminViewModel()
        {
            QuestionsGrid = new QuestionsGridViewModel
            {
                IsAdminView = true
            };
            Notification = new NotificationViewModel();
        }
    }
}
