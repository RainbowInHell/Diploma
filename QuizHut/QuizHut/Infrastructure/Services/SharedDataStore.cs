﻿namespace QuizHut.Infrastructure.Services
{
    using QuizHut.Infrastructure.EntityViewModels.Answers;
    using QuizHut.Infrastructure.EntityViewModels.Categories;
    using QuizHut.Infrastructure.EntityViewModels.Events;
    using QuizHut.Infrastructure.EntityViewModels.Groups;
    using QuizHut.Infrastructure.EntityViewModels.Questions;
    using QuizHut.Infrastructure.EntityViewModels.Quizzes;
    using QuizHut.Infrastructure.Services.Contracts;

    public class SharedDataStore : ISharedDataStore
    {
        public GroupListViewModel? SelectedGroup { get; set; }

        public CategoryViewModel? SelectedCategory { get; set; }

        public EventListViewModel? SelectedEvent { get; set; }

        public QuizListViewModel? SelectedQuiz { get; set; }

        public QuestionViewModel? SelectedQuestion { get; set; }

        public AnswerViewModel? SelectedAnswer { get; set; }

        public AttemptedQuizViewModel? QuizToPass { get; set; }

        public AttemptedQuizQuestionViewModel? CurrentQuestion { get; set; }

        public string CurrentResultId { get; set; }
    }
}