﻿namespace QuizHut.ViewModels.MainViewModels.TeacherPartViewModels.QuizViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using QuizHut.BLL.Services.Contracts;
    using QuizHut.Infrastructure.Commands;
    using QuizHut.Infrastructure.Commands.Base;
    using QuizHut.Infrastructure.Commands.Base.Contracts;
    using QuizHut.Infrastructure.Services.Contracts;
    using QuizHut.ViewModels.Base;

    class AddEditAnswerViewModel : ViewModel
    {
        private readonly IAnswersService answersService;

        private readonly ISharedDataStore sharedDataStore;

        private readonly IViewDisplayTypeService viewDisplayTypeService;

        public AddEditAnswerViewModel(
            IAnswersService answersService,
            ISharedDataStore sharedDataStore,
            IRenavigator newAnswerRenavigator,
            IRenavigator quizSettingsRenavigator,
            IRenavigator newQuestionRenavigator,
            IViewDisplayTypeService viewDisplayTypeService)
        {
            this.answersService = answersService;
            this.sharedDataStore = sharedDataStore;
            this.viewDisplayTypeService = viewDisplayTypeService;

            NavigateNewAnswerCommand = new RenavigateCommand(newAnswerRenavigator, ViewDisplayType.Create, viewDisplayTypeService);
            NavigateQuizSettingsCommand = new RenavigateCommand(quizSettingsRenavigator);
            NavigateNewQuestionCommand = new RenavigateCommand(newQuestionRenavigator, ViewDisplayType.Create, viewDisplayTypeService);

            CreateAnswerCommandAsync = new ActionCommandAsync(OnCreateAnswerCommandExecutedAsync, CanCreateUpdateAnswerCommandExecute);
            UpdateAnswerCommandAsync = new ActionCommandAsync(OnUpdateAnswerCommandExecutedAsync, CanCreateUpdateAnswerCommandExecute);
        }

        #region Fields and properties

        public ViewDisplayType? CurrentViewDisplayType
        {
            get
            {
                if (viewDisplayTypeService.CurrentViewDisplayType == ViewDisplayType.Edit)
                {
                    IsRightAnswer = sharedDataStore.SelectedAnswer.IsRightAnswer;
                    AnswerDescriptionToCreate = sharedDataStore.SelectedAnswer.Text;
                }

                return viewDisplayTypeService.CurrentViewDisplayType;
            }
        }

        private bool isRightAnswer;
        public bool IsRightAnswer
        {
            get => isRightAnswer;
            set => Set(ref isRightAnswer, value);
        }

        private string answerDescriptionToCreate;
        public string AnswerDescriptionToCreate
        {
            get => answerDescriptionToCreate;
            set => Set(ref answerDescriptionToCreate, value);
        }

        private string? createUpdateErrorMessage;
        public string? CreateUpdateErrorMessage
        {
            get => createUpdateErrorMessage;
            set => Set(ref createUpdateErrorMessage, value);
        }

        #endregion

        #region NavigationCommands

        public ICommand NavigateQuizSettingsCommand { get; }

        public ICommand NavigateNewQuestionCommand { get; }

        public ICommand NavigateNewAnswerCommand { get; }

        #endregion

        #region CreateAnswerCommandAsync

        public ICommandAsync CreateAnswerCommandAsync { get; }

        private bool CanCreateUpdateAnswerCommandExecute(object p)
        {
            if (string.IsNullOrEmpty(AnswerDescriptionToCreate))
            {
                CreateUpdateErrorMessage = "Все поля должны быть заполнены";
                return false;
            }

            CreateUpdateErrorMessage = null;
            return true;
        }

        private async Task OnCreateAnswerCommandExecutedAsync(object p)
        {
            await answersService.CreateAnswerAsync(AnswerDescriptionToCreate, IsRightAnswer, sharedDataStore.SelectedQuestion.Id);

            NavigateNewAnswerCommand.Execute(p);
        }

        #endregion

        #region UpdateAnswerCommandAsync

        public ICommandAsync UpdateAnswerCommandAsync { get; }

        private async Task OnUpdateAnswerCommandExecutedAsync(object p)
        {
            await answersService.UpdateAnswerAsync(sharedDataStore.SelectedAnswer.Id, AnswerDescriptionToCreate, IsRightAnswer);

            NavigateQuizSettingsCommand.Execute(p);
        }

        #endregion
    }
}