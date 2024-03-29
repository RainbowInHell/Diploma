﻿namespace QuizHut.ViewModels.MainViewModels.StudentPartViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using FontAwesome.Sharp;

    using QuizHut.BLL.Services.Contracts;
    using QuizHut.Infrastructure.Commands.Base;
    using QuizHut.Infrastructure.Commands.Base.Contracts;
    using QuizHut.Infrastructure.EntityViewModels.Results;
    using QuizHut.Infrastructure.Services.Contracts;
    using QuizHut.ViewModels.Base;
    using QuizHut.ViewModels.Contracts;

    class OwnResultsViewModel : ViewModel, IMenuView
    {
        public string Title { get; set; } = "Мои результаты";

        public IconChar IconChar { get; set; } = IconChar.Trophy;

        public Dictionary<string, string> SearchCriteriasInEnglish => new()
        {
            { "Название события", "EventName" },
            { "Название викторины", "QuizName" }
        };

        private readonly IResultsService resultsService;

        private readonly ISharedDataStore sharedDataStore;

        private readonly IExporter exporter;

        public OwnResultsViewModel(IResultsService resultsService, ISharedDataStore sharedDataStore, IExporter exporter)
        {
            this.resultsService = resultsService;
            this.sharedDataStore = sharedDataStore;
            this.exporter = exporter;

            LoadDataCommandAsync = new ActionCommandAsync(OnLoadDataCommandExecutedAsync);
            SearchCommandAsync = new ActionCommandAsync(OnSearchCommandAsyncExecute, CanSearchCommandAsyncExecute);
            ExportDataAsyncCommand = new ActionCommandAsync(OnExportDataAsyncCommandExecute);
        }

        #region Fields and properties

        public ObservableCollection<StudentResultViewModel> studentResults;
        public ObservableCollection<StudentResultViewModel> StudentResults
        {
            get => studentResults;
            set => Set(ref studentResults, value);
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

        #endregion

        #region SearchCommandAsync

        public ICommandAsync SearchCommandAsync { get; }

        private bool CanSearchCommandAsyncExecute(object p) => SearchCriteria != null;

        private async Task OnSearchCommandAsyncExecute(object p)
        {
            await LoadStudentResultsAsync(SearchCriteriasInEnglish[SearchCriteria], SearchText);
        }

        #endregion

        #region LoadDataCommandAsync

        public ICommandAsync LoadDataCommandAsync { get; }

        private async Task OnLoadDataCommandExecutedAsync(object p)
        {
            await LoadStudentResultsAsync();
        }

        #endregion

        #region ExportDataCommand

        public ICommandAsync ExportDataAsyncCommand { get; }

        private async Task OnExportDataAsyncCommandExecute(object p)
        {
            string fullName = $"{sharedDataStore.CurrentUser.LastName} {sharedDataStore.CurrentUser.FirstName}";
            await exporter.GenerateExcelReportAsync(studentResults, fullName);
        }

        #endregion

        private async Task LoadStudentResultsAsync(string searchCriteria = null, string searchText = null)
        {
            var studentResults = await resultsService.GetAllResultsByStudentIdAsync<StudentResultViewModel>(sharedDataStore.CurrentUser.Id, searchCriteria, searchText);

            StudentResults = new(studentResults);
        }
    }
}