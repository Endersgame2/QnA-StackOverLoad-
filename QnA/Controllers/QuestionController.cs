using Microsoft.AspNetCore.Mvc;
using QnA.Attributes;
using QnA.Models.Data.Shared;
using QnA.Models.Question;
using Microsoft.AspNetCore.Mvc.Rendering;
using QnA.Models.Data;
using QnA.Enums;

namespace QnA.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ILogger<QuestionController> _logger;
        private readonly QnADbContext _qnADbContext;
        public QuestionController(ILogger<QuestionController> logger, QnADbContext qnADbContext)
        {
            _logger = logger;
            _qnADbContext = qnADbContext;
        }

        [HttpGet, Route("Question/{id:int}")]
        public IActionResult Index([FromRoute] int id)
        {
            var model = PrepareQuestionViewModel(id);
            return View(model);
        }

        [HttpPost, QnAAuthorize("User")]
        public IActionResult AddAnswer([FromForm] QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                if (model.QuestionId == 0)
                {
                    return BadRequest();
                }
                model = PrepareQuestionViewModel((int)model.QuestionId);
                model.Notification.Message = "Validation failed";
                model.Notification.MessageClass = "alert-danger";
                return View("Index", model);
            }
            _qnADbContext.Answers.Add(new Answer()
            {
                QuestionId = model.QuestionId,
                Text = model.Answer,
                UserId = (int)HttpContext.Session.GetInt32("UserId")
            });
            _qnADbContext.SaveChanges();
            return ReloadWithNotification(model.QuestionId, "Answer added", "alert-success");
        }

        [HttpPost, QnAAuthorize("User")]
        public IActionResult AddComment([FromForm] CommentViewModel model)
        {
            var questionViewModel = new QuestionViewModel();
            if (!ModelState.IsValid)
            {
                if(model.QuestionId == 0)
                {
                    return BadRequest();
                }
                return ReloadWithNotification(model.QuestionId, "Validation failed", "alert-danger");
            }
            _qnADbContext.Comments.Add(new Comment()
            {
                QuestionId = (int)model.QuestionId,
                AnswerId = model.AnswerId,
                Text = model.Comment,
                UserId = GetUserId()
            });
            _qnADbContext.SaveChanges();
            return ReloadWithNotification(model.QuestionId, "Comment added", "alert-success");
        }

        [HttpGet, QnAAuthorize("User")]
        public IActionResult Add()
        {
            var tags = _qnADbContext.Tags.ToList();
            var model = new AddQuestionViewModel
            {
                TagOptions = new MultiSelectList(tags, "Id", "Name")
            };
            return View(model);
        }
        [HttpPost, QnAAuthorize("User")]
        public IActionResult Add([FromForm] AddQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Notification.Message = "Validation failed";
                model.Notification.MessageClass = "alert-danger";
                return View(model);
            }
            var userId = GetUserId();
            try
            {
                _qnADbContext.Database.BeginTransaction();
                var question = new Question()
                {
                    Title = model.Title,
                    Text = model.Text,
                    UserId = userId,
                    TimeStampUtc = DateTime.UtcNow
                };
                _qnADbContext.Questions.Add(question);
                if(model.TagIds != null && model.TagIds.Any())
                {
                    _qnADbContext.QuestionTags.AddRange(model.TagIds.Select(tagId => new QuestionTag()
                    {
                        QuestionId = question.Id,
                        TagId = tagId
                    }));
                }
                _qnADbContext.SaveChanges();
                _qnADbContext.Database.CurrentTransaction.Commit();
                // TO-DO: Redirect to question detail
                return RedirectToAction("Index", "Home");
            }
            catch(Exception e)
            {
                _logger.LogError($"Failed to add question by user ${userId}: ${model.Title}", e);
                _qnADbContext.Database.CurrentTransaction.Rollback();
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost, QnAAuthorize("User")]
        public IActionResult Vote([FromForm] VoteActionViewModel model)
        {
            var questionViewModel = new QuestionViewModel();
            var userId = GetUserId();
            if (!ModelState.IsValid)
            {
                if (model.QuestionId == 0)
                {
                    return BadRequest();
                }
                return ReloadWithNotification(model.QuestionId, "Validation failed", "alert-danger");
            }
            var existingVote = _qnADbContext.Votes.FirstOrDefault(x => x.QuestionId == model.QuestionId && x.AnswerId == model.AnswerId && x.UserId == userId);
            if (model.AnswerId == 0 || model.AnswerId == null)
            {
                var question = _qnADbContext.Questions.FirstOrDefault(x => x.Id == model.QuestionId);
                if (question.UserId == userId)
                {
                    return ReloadWithNotification(model.QuestionId, "You may not upvote your own question", "alert-danger");
                }
                var askedBy = _qnADbContext.Users.FirstOrDefault(x => x.Id == question.UserId);
                if (askedBy.Reputation == null)
                {
                    askedBy.Reputation = 0;
                }
                if (existingVote == null) // add new vote
                {
                    askedBy.Reputation += (5 * (int)model.VoteType);
                    _qnADbContext.Votes.Add(new Vote()
                    {
                        QuestionId = model.QuestionId,
                        AnswerId = null,
                        UserId = userId,
                        VoteType = model.VoteType
                    });
                }
                else if(existingVote.VoteType == VoteType.None) // add removed vote again
                {
                    askedBy.Reputation += (5 * (int)model.VoteType);
                    existingVote.VoteType = model.VoteType;
                }
                else if (existingVote.VoteType == model.VoteType) // remove old vote
                {
                    askedBy.Reputation -= (5 * (int)model.VoteType);
                    existingVote.VoteType = VoteType.None;
                }
                else // change vote type
                {
                    askedBy.Reputation += (2 * 5 * (int)model.VoteType);
                    existingVote.VoteType = model.VoteType;
                }

            }
            else
            {
                var answer = _qnADbContext.Answers.FirstOrDefault(x => x.Id == model.AnswerId);
                if (answer.UserId == userId)
                {
                    return ReloadWithNotification(model.QuestionId, "You may not upvote your own answer", "alert-danger");
                }
                var answeredBy = _qnADbContext.Users.FirstOrDefault(x => x.Id == answer.UserId);
                if (answeredBy.Reputation == null)
                {
                    answeredBy.Reputation = 0;
                }
                if (existingVote == null) // add new vote
                {
                    answeredBy.Reputation += (5 * (int)model.VoteType);
                    _qnADbContext.Votes.Add(new Vote()
                    {
                        QuestionId = model.QuestionId,
                        AnswerId = model.AnswerId,
                        UserId = userId,
                        VoteType = model.VoteType
                    }); ;
                }
                else if (existingVote.VoteType == VoteType.None)// update previously removed vote
                {
                    answeredBy.Reputation += (5 * (int)model.VoteType);
                    existingVote.VoteType = model.VoteType;
                }
                else if (existingVote.VoteType == model.VoteType) // remove old vote
                {
                    answeredBy.Reputation -= (5 * (int)model.VoteType);
                    existingVote.VoteType = VoteType.None;
                }
                else // change vote type
                {
                    answeredBy.Reputation += (2 * 5 * (int)model.VoteType);
                    existingVote.VoteType = model.VoteType;
                }
            }
            try
            {
                _qnADbContext.Database.BeginTransaction();
                _qnADbContext.SaveChanges();
                _qnADbContext.Database.CurrentTransaction.Commit();
                questionViewModel = PrepareQuestionViewModel((int)model.QuestionId);
                return View("Index", questionViewModel);
            }
            catch(Exception e)
            {
                _logger.LogError($"Failed to update vote by user ${userId}", e);
                _qnADbContext.Database.CurrentTransaction.Rollback();
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost, QnAAuthorize("User")]
        public IActionResult AcceptAnswer([FromForm] int answerId)
        {
            var answer = _qnADbContext.Answers.FirstOrDefault(x => x.Id == answerId);
            var question = _qnADbContext.Questions.FirstOrDefault(x => x.Id == answer.QuestionId);
            if(question.UserId != GetUserId())
            {
                return Forbid();
            }
            question.AcceptedAnswerId = answerId;
            _qnADbContext.SaveChanges();
            return ReloadWithNotification(question.Id, "Answer accepted", "alert-success");
        }
        private IActionResult ReloadWithNotification(int? questionId, string message, string notifClass)
        {
            var questionViewModel = PrepareQuestionViewModel((int)questionId);
            questionViewModel.Notification.Message = message;
            questionViewModel.Notification.MessageClass = notifClass;
            return View("Index", questionViewModel);
        }
        private QuestionViewModel PrepareQuestionViewModel(int id)
        {
            var userId = GetUserId();
            var model = (from question in _qnADbContext.Questions
                         join user in _qnADbContext.Users on question.UserId equals user.Id
                         where question.Id == id
                         select new QuestionViewModel()
                         {
                             QuestionId = question.Id,
                             Title = question.Title,
                             Text = question.Text,
                             User = user.Name + " (" + (user.Reputation != null ? user.Reputation : 0) + ")",
                             UpVoteCount = (from vote in _qnADbContext.Votes where vote.VoteType == VoteType.Up && vote.QuestionId == id && vote.AnswerId == null select 1).Count(),
                             DownVoteCount = (from vote in _qnADbContext.Votes where vote.VoteType == VoteType.Down && vote.QuestionId == id && vote.AnswerId == null select 1).Count(),
                             CurrentUserVoteType = (from vote in _qnADbContext.Votes where vote.QuestionId == id && vote.AnswerId == null && vote.UserId == userId select vote.VoteType).FirstOrDefault(),
                             IsCurrentUserAuthor = question.UserId == userId
                         }).FirstOrDefault();
            model.CommentDetails.Comments = (from comment in _qnADbContext.Comments
                                             join user in _qnADbContext.Users on comment.UserId equals user.Id
                                             where comment.QuestionId == id && comment.AnswerId == null
                                             select new CommentViewModel()
                                             {
                                                 Comment = comment.Text,
                                                 User = user.Name + " (" + (user.Reputation != null ? user.Reputation : 0) + ")"
                                             }).ToList();
            model.CommentDetails.AddComment.QuestionId = id;
            model.Tags = (from questionTag in _qnADbContext.QuestionTags
                          join tag in _qnADbContext.Tags on questionTag.TagId equals tag.Id
                          where questionTag.QuestionId == id
                          select tag.Name).ToList();
            model.Answers = (from answer in _qnADbContext.Answers
                             join user in _qnADbContext.Users on answer.UserId equals user.Id
                             where answer.QuestionId == id
                             select new AnswerViewModel()
                             {
                                 QuestionId = id,
                                 AnswerId = answer.Id,
                                 Answer = answer.Text,
                                 User = user.Name + " (" + (user.Reputation != null ? user.Reputation : 0) + ")",
                                 UpVoteCount = (from vote in _qnADbContext.Votes where vote.VoteType == VoteType.Up && vote.QuestionId == id && vote.AnswerId == answer.Id select 1).Count(),
                                 DownVoteCount = (from vote in _qnADbContext.Votes where vote.VoteType == VoteType.Down && vote.QuestionId == id && vote.AnswerId == answer.Id select 1).Count(),
                                 CurrentUserVoteType = (from vote in _qnADbContext.Votes where vote.QuestionId == id && vote.AnswerId == answer.Id && vote.UserId == userId select vote.VoteType).FirstOrDefault(),
                             }).ToList();
            foreach (var answer in model.Answers)
            {

                answer.CommentDetails.Comments = (from comment in _qnADbContext.Comments
                                   join user in _qnADbContext.Users on comment.UserId equals user.Id
                                   where comment.QuestionId == id && comment.AnswerId == answer.AnswerId
                                   select new CommentViewModel()
                                   {
                                       Comment = comment.Text,
                                       User = user.Name + " (" + (user.Reputation != null ? user.Reputation : 0) + ")",
                                   }).ToList();
                answer.CommentDetails.AddComment.QuestionId = id;
                answer.CommentDetails.AddComment.AnswerId = answer.AnswerId;
            }
            var acceptedAnswerId = _qnADbContext.Questions.Where(x => x.Id == id).Select(y => y.AcceptedAnswerId).FirstOrDefault();
            if (acceptedAnswerId != null && acceptedAnswerId > 0)
            {
                var acceptedAnswerIndex = model.Answers.FindIndex(x => x.AnswerId == acceptedAnswerId);
                if(acceptedAnswerIndex >= 0)
                {
                    model.AcceptedAnswer = model.Answers[acceptedAnswerIndex];
                    model.Answers.RemoveAt(acceptedAnswerIndex);
                }
            }
            return model;
        }
        private int GetUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return userId != null ? (int)userId : 0;
        }
    }
}
