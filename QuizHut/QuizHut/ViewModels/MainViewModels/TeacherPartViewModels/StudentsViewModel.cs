﻿namespace QuizHut.ViewModels.MainViewModels.TeacherPartViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
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
        public string Title { get; set; } = "Участники";

        public IconChar IconChar { get; set; } = IconChar.UserGroup;

        public Dictionary<string, string> SearchCriteriasInEnglish => new()
        {
            { "Полное имя", "FullName" },
            { "Имя", "FirstName" },
            { "Фамилия", "LastName" },
            { "Почта", "Email" }
        };

        private readonly IStudentsService studentService;

        private readonly ISharedDataStore sharedDataStore;

        private readonly IExporter exporter;

        public StudentsViewModel(IStudentsService studentService, IExporter exporter, ISharedDataStore sharedDataStore)
        {
            this.studentService = studentService;
            this.exporter = exporter;
            this.sharedDataStore = sharedDataStore;

            LoadDataCommandAsync = new ActionCommandAsync(OnLoadDataCommandExecutedAsync);
            SearchCommandAsync = new ActionCommandAsync(OnSearchCommandAsyncExecute, CanSearchCommandAsyncExecute);
            RefreshSearchCommandAsync = new ActionCommandAsync(OnRefreshSearchCommandAsyncExecute);
            AddStudentToTeacherListCommandAsync = new ActionCommandAsync(OnAddStudentToTeacherListCommandExecute, CanAddStudentToTeacherListCommandExecute);
            DeleteStudentFromTeacherListCommandAsync = new ActionCommandAsync(OnDeleteStudentFromTeacherListCommandExecute);
            ExportDataAsyncCommand = new ActionCommandAsync(OnExportDataAsyncExecute);
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

        private async Task OnLoadDataCommandExecutedAsync(object p)
        {
            await LoadStudentsDataAsync();
        }

        #endregion

        #region SearchCommandAsync

        public ICommandAsync SearchCommandAsync { get; }

        private bool CanSearchCommandAsyncExecute(object p) => !string.IsNullOrEmpty(SearchCriteria)  && !string.IsNullOrEmpty(SearchText);

        private async Task OnSearchCommandAsyncExecute(object p)
        {
            await LoadStudentsDataAsync(SearchCriteriasInEnglish[SearchCriteria], SearchText);
        }

        #endregion

        #region RefreshSearchCommandAsync

        public ICommandAsync RefreshSearchCommandAsync { get; }

        private async Task OnRefreshSearchCommandAsyncExecute(object p)
        {
            SearchCriteria = null;
            SearchText = null;

            await LoadStudentsDataAsync();
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
            else
            {
                ErrorMessage = "Участник не найден";
            }
        }

        #endregion

        #region DeleteStudentFromTeacherListCommandAsync

        public ICommandAsync DeleteStudentFromTeacherListCommandAsync { get; }

        private async Task OnDeleteStudentFromTeacherListCommandExecute(object p)
        {
            await studentService.DeleteStudentFromTeacherListAsync(SelectedStudent.Id, sharedDataStore.CurrentUser.Id);

            await LoadStudentsDataAsync();
        }

        #endregion

        #region ExportDataAsyncCommand

        public ICommandAsync ExportDataAsyncCommand { get; }

        private async Task OnExportDataAsyncExecute(object p)
        {
            await exporter.GenerateExcelReportAsync(Students);
        }

        #endregion

        private async Task LoadStudentsDataAsync(string searchCriteria = null, string searchText = null)
        {
            var students = await studentService.GetAllStudentsAsync<StudentViewModel>(sharedDataStore.CurrentUser.Id, searchCriteria, searchText);

            if (!students.Any())
            {
                ErrorMessage = "Участники не найдены";
            }
            else
            {
                ErrorMessage = null;
            }

            Students = new(students);
        }
    }
}