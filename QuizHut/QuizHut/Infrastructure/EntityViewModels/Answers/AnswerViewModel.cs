﻿namespace QuizHut.Infrastructure.EntityViewModels.Answers
{
    using QuizHut.BLL.MapperConfig.Contracts;
    using QuizHut.DLL.Entities;

    public class AnswerViewModel : IMapFrom<Answer>
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public bool IsRightAnswer { get; set; }

        public string QuestionId { get; set; }
    }
}