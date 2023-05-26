﻿namespace QuizHut.ViewModels.MainViewModels.TeacherPartViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using FontAwesome.Sharp;

    using QuizHut.BLL.Services.Contracts;
    using QuizHut.Infrastructure.Commands.Base;
    using QuizHut.Infrastructure.Commands.Base.Contracts;
    using QuizHut.Infrastructure.EntityViewModels;
    using QuizHut.Infrastructure.Services.Contracts;
    using QuizHut.ViewModels.Base;
    using QuizHut.ViewModels.Contracts;

    class StudentsViewModel : ViewModel, IMenuView
    {
        public static string Title { get; } = "Учащиеся";

        public static IconChar IconChar { get; } = IconChar.UserGroup;

        public Dictionary<string, string> SearchCriteriasInEnglish => new()
        {
            { "Полное имя", "FullName" },
            { "Имя", "FirstName" },
            { "Фамиля", "LastName" },
            { "Почта", "Email" }
        };

        private readonly IStudentsService studentService;

        private readonly ISharedDataStore sharedDataStore;

        public StudentsViewModel(IStudentsService studentService, ISharedDataStore sharedDataStore)
        {
            this.studentService = studentService;
            this.sharedDataStore = sharedDataStore;

            LoadDataCommandAsync = new ActionCommandAsync(OnLoadDataCommandExecutedAsync, CanLoadDataCommandExecute);
            AddStudentToTeacherListCommandAsync = new ActionCommandAsync(OnAddStudentToTeacherListCommandExecute, CanAddStudentToTeacherListCommandExecute);
            DeleteStudentFromTeacherListCommandAsync = new ActionCommandAsync(OnDeleteStudentFromTeacherListCommandExecute, CanDeleteStudentFromTeacherListCommandExecute);
            SearchCommandAsync = new ActionCommandAsync(OnSearchCommandAsyncExecute, CanSearchCommandAsyncExecute);
        }

        #region FieldsAndProperties

        private string studentEmailToAdd;
        public string StudentEmailToAdd
        {
            get => studentEmailToAdd;
            set => Set(ref studentEmailToAdd, value);
        }

        private StudentViewModel selectedStudent;
        public StudentViewModel SelectedStudent
        {
            get => selectedStudent;
            set => Set(ref selectedStudent, value);
        }

        public ObservableCollection<StudentViewModel> students;
        public ObservableCollection<StudentViewModel> Students
        {
            get => students;
            set => Set(ref students, value);
        }

        private string searchCriteria;
        public string SearchCriteria
        {
            get => searchCriteria;
            set => Set(ref searchCriteria, value);
        }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set => Set(ref searchText, value);
        }

        private string? errorMessage;
        public string? ErrorMessage
        {
            get => errorMessage;
            set => Set(ref errorMessage, value);
        }

        #endregion

        #region LoadDataCommandAsync

        public ICommandAsync LoadDataCommandAsync { get; }

        private bool CanLoadDataCommandExecute(object p) => true;

        private async Task OnLoadDataCommandExecutedAsync(object p)
        {
            await LoadStudentsDataAsync();
        }

        #endregion

        #region SearchCommandAsync

        public ICommandAsync SearchCommandAsync { get; }

        private bool CanSearchCommandAsyncExecute(object p) => true;

        private async Task OnSearchCommandAsyncExecute(object p)
        {
            await LoadStudentsDataAsync(SearchCriteriasInEnglish[SearchCriteria], SearchText);
        }

        #endregion

        #region AddStudentToTeacherListCommandAsync

        public ICommandAsync AddStudentToTeacherListCommandAsync { get; }

        private bool CanAddStudentToTeacherListCommandExecute(object p) => !string.IsNullOrEmpty(StudentEmailToAdd);

        private async Task OnAddStudentToTeacherListCommandExecute(object p)
        {
            var partisipantIsAdded = await studentService.AddStudentAsync(StudentEmailToAdd, sharedDataStore.CurrentUser.Id);

            if (partisipantIsAdded)
            {
                await LoadStudentsDataAsync();
            }
        }

        #endregion

        #region DeleteStudentFromTeacherListCommandAsync

        public ICommandAsync DeleteStudentFromTeacherListCommandAsync { get; }

        private bool CanDeleteStudentFromTeacherListCommandExecute(object p) => true;

        private async Task OnDeleteStudentFromTeacherListCommandExecute(object p)
        {
            await studentService.DeleteStudentFromTeacherListAsync(SelectedStudent.Id, sharedDataStore.CurrentUser.Id);

            await LoadStudentsDataAsync();
        }

        #endregion

        private async Task LoadStudentsDataAsync(string searchCriteria = null, string searchText = null)
        {
            var students = await studentService.GetAllStudentsAsync<StudentViewModel>(sharedDataStore.CurrentUser.Id, searchCriteria: searchCriteria, searchText: searchText);

            Students = new(students);
        }
    }
}