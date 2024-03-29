﻿namespace QuizHut.Infrastructure.Services.Contracts
{
    using System;
    using QuizHut.BLL.Helpers.Contracts;
    using System.Windows.Threading;

    using QuizHut.DLL.Entities;
    using QuizHut.Infrastructure.EntityViewModels.Answers;
    using QuizHut.Infrastructure.EntityViewModels.Categories;
    using QuizHut.Infrastructure.EntityViewModels.Events;
    using QuizHut.Infrastructure.EntityViewModels.Groups;
    using QuizHut.Infrastructure.EntityViewModels.Questions;
    using QuizHut.Infrastructure.EntityViewModels.Quizzes;
    
    public interface ISharedDataStore
    {
        ApplicationUser? CurrentUser { get; set; }

        UserRole? CurrentUserRole { get; set; }

        GroupListViewModel? SelectedGroup { get; set; }

        CategoryViewModel? SelectedCategory { get; set; }

        EventListViewModel? SelectedEvent { get; set; }

        QuizListViewModel? SelectedQuiz { get; set; }

        QuestionViewModel? SelectedQuestion { get; set; }

        AnswerViewModel? SelectedAnswer { get; set; }

        AttemptedQuizViewModel? QuizToPass { get; set; }

        AttemptedQuizQuestionViewModel? CurrentQuestion { get; set; }

        string CurrentResultId { get; set; }

        TimeSpan RemainingTime { get; set; }

        EventSimpleViewModel? EventToView { get; set; }

        DispatcherTimer DispatcherTimer { get; set; }
    }
}