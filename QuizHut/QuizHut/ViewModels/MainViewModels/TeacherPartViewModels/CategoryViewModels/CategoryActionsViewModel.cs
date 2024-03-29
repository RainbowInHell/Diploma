﻿namespace QuizHut.ViewModels.MainViewModels.TeacherPartViewModels.CategoryViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using QuizHut.BLL.Services.Contracts;
    using QuizHut.Infrastructure.Commands;
    using QuizHut.Infrastructure.Commands.Base;
    using QuizHut.Infrastructure.Commands.Base.Contracts;
    using QuizHut.Infrastructure.EntityViewModels.Quizzes;
    using QuizHut.Infrastructure.Services.Contracts;
    using QuizHut.ViewModels.Base;

    class CategoryActionsViewModel : ViewModel
    {
        private readonly ICategoriesService categoriesService;

        private readonly IQuizzesService quizzesService;

        private readonly ISharedDataStore sharedDataStore;

        private readonly IViewDisplayTypeService viewDisplayTypeService;

        public CategoryActionsViewModel(
            ICategoriesService categoriesService,
            IQuizzesService quizzesService,
            ISharedDataStore sharedDataStore,
            IRenavigator categoryRenavigator,
            IRenavigator categorySettingRenavigator,
            IRenavigator addQuizRenavigator,
            IViewDisplayTypeService viewDisplayTypeService)
        {
            this.categoriesService = categoriesService;
            this.quizzesService = quizzesService;
            this.sharedDataStore = sharedDataStore;
            this.viewDisplayTypeService = viewDisplayTypeService;

            viewDisplayTypeService.StateChanged += ViewDisplayTypeService_StateChanged;

            NavigateCategoryCommand = new RenavigateCommand(categoryRenavigator);
            NavigateCategorySettingsCommand = new RenavigateCommand(categorySettingRenavigator);
            NavigateAddQuizCommand = new RenavigateCommand(addQuizRenavigator, ViewDisplayType.Create, viewDisplayTypeService);

            LoadDataCommandAsync = new ActionCommandAsync(OnLoadDataCommandExecutedAsync, CanLoadDataCommandExecute);
            CreateCategoryCommandAsync = new ActionCommandAsync(OnCreateCategoryCommandExecutedAsync, CanCreateUpdateCategoryNameCommandExecute);
            UpdateCategoryNameCommandAsync = new ActionCommandAsync(OnUpdateCategoryNameCommandExecutedAsync, CanCreateUpdateCategoryNameCommandExecute);
            AssignQuizzesToCategoryCommandAsync = new ActionCommandAsync(OnAssignQuizzesToCategoryCommandExecute, CanAssignQuizzesToCategoryCommandExecute);
        }

        #region Fields and properties

        private bool isQuizzesEmpty;
        public bool IsQuizzesEmpty
        {
            get => isQuizzesEmpty;
            set => Set(ref isQuizzesEmpty, value);
        }

        public ViewDisplayType? CurrentViewDisplayType
        {
            get
            {
                if (viewDisplayTypeService.CurrentViewDisplayType == ViewDisplayType.Edit)
                {
                    CategoryNameToCreate = sharedDataStore.SelectedCategory.Name;
                }

                return viewDisplayTypeService.CurrentViewDisplayType;
            }
        }

        private string categoryNameToCreate;
        public string CategoryNameToCreate
        {
            get => categoryNameToCreate;
            set => Set(ref categoryNameToCreate, value);
        }

        public ObservableCollection<QuizAssignViewModel> quizzes = new ();
        public ObservableCollection<QuizAssignViewModel> Quizzes
        {
            get => quizzes;
            set => Set(ref quizzes, value);
        }

        private string? createUpdateErrorMessage;
        public string? CreateUpdateErrorMessage
        {
            get => createUpdateErrorMessage;
            set => Set(ref createUpdateErrorMessage, value);
        }

        private string? errorMessageQuizzes;
        public string? ErrorMessageQuizzes
        {
            get => errorMessageQuizzes;
            set => Set(ref errorMessageQuizzes, value);
        }

        #endregion

        #region NavigationCommands

        public ICommand NavigateCategoryCommand { get; }

        public ICommand NavigateCategorySettingsCommand { get; }

        public ICommand NavigateAddQuizCommand { get; }

        #endregion

        #region LoadDataCommandAsync

        public ICommandAsync LoadDataCommandAsync { get; }

        private bool CanLoadDataCommandExecute(object p)
        {
            if (CurrentViewDisplayType == ViewDisplayType.AddQuizzes)
            {
                return true;
            }

            return false;
        }

        private async Task OnLoadDataCommandExecutedAsync(object p)
        {
            await LoadQuizzesData();
        }

        #endregion

        #region CreateCategoryCommandAsync

        public ICommandAsync CreateCategoryCommandAsync { get; }

        private bool CanCreateUpdateCategoryNameCommandExecute(object p)
        {
            if (string.IsNullOrEmpty(CategoryNameToCreate))
            {
                CreateUpdateErrorMessage = "Название категории не может быть пустым";
                return false;
            }

            CreateUpdateErrorMessage = null;
            return true;
        }

        private async Task OnCreateCategoryCommandExecutedAsync(object p)
        {
            await categoriesService.CreateCategoryAsync(CategoryNameToCreate, sharedDataStore.CurrentUser.Id);

            NavigateCategoryCommand.Execute(p);
        }

        #endregion

        #region UpdateCategoryNameCommandAsync

        public ICommandAsync UpdateCategoryNameCommandAsync { get; }

        private async Task OnUpdateCategoryNameCommandExecutedAsync(object p)
        {
            await categoriesService.UpdateCategoryNameAsync(sharedDataStore.SelectedCategory.Id, CategoryNameToCreate);

            NavigateCategoryCommand.Execute(p);
        }

        #endregion

        #region AssignQuizzesToCategoryCommandAsync

        public ICommandAsync AssignQuizzesToCategoryCommandAsync { get; }

        private bool CanAssignQuizzesToCategoryCommandExecute(object p)
        {
            if (!Quizzes.Where(s => s.IsAssigned).Any())
            {
                ErrorMessageQuizzes = "Выберите хотя бы одну викторину";
                return false;
            }

            ErrorMessageQuizzes = null;
            return true;
        }

        private async Task OnAssignQuizzesToCategoryCommandExecute(object p)
        {
            var selectedQuizIds = Quizzes.Where(s => s.IsAssigned).Select(s => s.Id).ToList();

            await categoriesService.AssignQuizzesToCategoryAsync(sharedDataStore.SelectedCategory.Id, selectedQuizIds);

            NavigateCategorySettingsCommand.Execute(p);
        }

        #endregion

        private async Task LoadQuizzesData()
        {
            var quizzes = await quizzesService.GetUnAssignedQuizzesToCategoryByCreatorIdAsync<QuizAssignViewModel>(sharedDataStore.CurrentUser.Id);

            IsQuizzesEmpty = !quizzes.Any();

            if (!IsQuizzesEmpty)
            {
                Quizzes = new(quizzes);
            }
        }

        private void ViewDisplayTypeService_StateChanged()
        {
            OnPropertyChanged(nameof(CurrentViewDisplayType));
        }

        public override void Dispose()
        {
            viewDisplayTypeService.StateChanged -= ViewDisplayTypeService_StateChanged;

            base.Dispose();
        }
    }
}